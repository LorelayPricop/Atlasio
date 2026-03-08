import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiClient, DestinationDto, DestinationType, UpdateDestinationDto } from '../services/api-client';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { COUNTRY_OPTIONS, getCountryNameByCode } from '../shared/enums/country.enum';
import { DESTINATION_TYPE_OPTIONS } from '../shared/enums/destination-type.enum';

@Component({
  selector: 'app-edit-destination-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent],
  templateUrl: './edit-destination-page.component.html',
  styleUrls: ['./edit-destination-page.component.css']
})
export class EditDestinationPageComponent implements OnInit {
  private readonly apiService = inject(ApiClient);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  destination = signal<DestinationDto | null>(null);
  countries = signal<string[]>(COUNTRY_OPTIONS);
  destinationTypeOptions = DESTINATION_TYPE_OPTIONS;
  selectedImageName = signal<string>('');
  selectedImageDataUrl = signal<string>('');
  imageRemoved = signal<boolean>(false);

  formModel: {
    id: number;
    name: string;
    countryCode: string;
    type: DestinationType | null;
    description: string;
    longDescription: string;
  } = {
    id: 0,
    name: '',
    countryCode: '',
    type: null,
    description: '',
    longDescription: ''
  };

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.showAlert('error', 'Invalid destination ID.');
      return;
    }

    const id = parseInt(idParam, 10);
    if (Number.isNaN(id)) {
      this.showAlert('error', 'Invalid destination ID.');
      return;
    }

    this.loadCountries();
    this.loadDestination(id);
  }

  loadCountries(): void {
    this.apiService.countries().subscribe({
      next: (countries: string[]) => {
        const merged = new Set<string>([...COUNTRY_OPTIONS, ...(countries || [])]);
        this.countries.set(Array.from(merged));
      },
      error: () => {
        this.countries.set(COUNTRY_OPTIONS);
      }
    });
  }

  loadDestination(id: number): void {
    this.loading.set(true);
    this.hideAlert();

    this.apiService.destinationsGET2(id).subscribe({
      next: (dest: DestinationDto) => {
        this.destination.set(dest);
        this.formModel = {
          id: dest.id,
          name: dest.name || '',
          countryCode: dest.countryCode || '',
          type: dest.type,
          description: dest.description || '',
          longDescription: dest.longDescription || ''
        };

        this.selectedImageDataUrl.set(dest.imageUrl || '');
        this.selectedImageName.set(dest.imageUrl ? 'Current image' : '');
        this.imageRemoved.set(false);
        this.loading.set(false);
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error loading destination: ' + (error?.message || 'Unknown error'));
      }
    });
  }

  getCountryDisplayName(countryCode?: string): string {
    return getCountryNameByCode(countryCode) || countryCode || 'Not specified';
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;

    if (!file) {
      return;
    }

    const maxSizeBytes = 10 * 1024 * 1024;
    if (file.size > maxSizeBytes) {
      this.showAlert('warning', 'Image exceeds 10MB limit. Please choose a smaller file.');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const result = typeof reader.result === 'string' ? reader.result : '';
      this.selectedImageName.set(file.name);
      this.selectedImageDataUrl.set(result);
      this.imageRemoved.set(false);
    };
    reader.onerror = () => {
      this.showAlert('error', 'Error reading selected image.');
    };

    reader.readAsDataURL(file);
  }

  onRemoveImage(): void {
    this.selectedImageName.set('');
    this.selectedImageDataUrl.set('');
    this.imageRemoved.set(true);
  }

  onCancel(): void {
    this.router.navigate(['/destinations', this.formModel.id]);
  }

  onSubmit(): void {
    const model = this.formModel;
    if (!model.name.trim() || !model.description.trim() || !model.countryCode || model.type === null) {
      this.showAlert('warning', 'Please complete all required fields.');
      return;
    }

    const dto = new UpdateDestinationDto();
    dto.name = model.name.trim();
    dto.description = model.description.trim();
    dto.longDescription = model.longDescription.trim() || undefined;
    dto.countryCode = model.countryCode;
    dto.type = model.type;

    if (this.imageRemoved()) {
      (dto as any).imageUrl = null;
    } else {
      dto.imageUrl = this.selectedImageDataUrl() || undefined;
    }

    this.loading.set(true);
    this.hideAlert();

    this.apiService.destinationsPUT(model.id, dto).subscribe({
      next: (updated) => {
        this.loading.set(false);
        this.showAlert('success', `Destination "${updated.name}" updated successfully.`);
        setTimeout(() => this.router.navigate(['/destinations', updated.id]), 700);
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error updating destination: ' + (error?.message || 'Unknown error'));
      }
    });
  }

  showAlert(type: string, message: string): void {
    this.alert.set({ show: true, type, message });
  }

  hideAlert(): void {
    this.alert.set({ show: false, type: 'info', message: '' });
  }

  onAlertDismissed(): void {
    this.hideAlert();
  }
}
