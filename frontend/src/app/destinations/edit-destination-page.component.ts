import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApiClient, DestinationDto, UpdateDestinationDto, CountryDto, DestinationTypeDto } from '../services/api-client';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { CatalogService } from '../services/catalog.service';

@Component({
  selector: 'app-edit-destination-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent],
  templateUrl: './edit-destination-page.component.html',
  styleUrls: ['./edit-destination-page.component.css']
})
export class EditDestinationPageComponent implements OnInit {
  private readonly apiService = inject(ApiClient);
  private readonly catalogService = inject(CatalogService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  destination = signal<DestinationDto | null>(null);
  // Catálogos cargados del backend
  countries = signal<CountryDto[]>([]);
  destinationTypes = signal<DestinationTypeDto[]>([]);
  selectedImageName = signal<string>('');
  selectedImageDataUrl = signal<string>('');
  imageRemoved = signal<boolean>(false);

  formModel: {
    id: number;
    name: string;
    countryCode: string;
    destinationTypeId: number | null;
    description: string;
    longDescription: string;
  } = {
    id: 0,
    name: '',
    countryCode: '',
    destinationTypeId: null,
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

    this.loadCatalogs();
    this.loadDestination(id);
  }

  /**
   * Carga catálogos de países y tipos de destino desde el backend
   */
  loadCatalogs(): void {
    // Cargar países
    this.catalogService.getCountries().subscribe({
      next: (countries) => {
        this.countries.set(countries);
      },
      error: (error) => {
        console.error('Error loading countries:', error);
        this.showAlert('warning', 'Could not load countries catalog.');
      }
    });

    // Cargar tipos de destino
    this.catalogService.getDestinationTypes().subscribe({
      next: (types) => {
        this.destinationTypes.set(types);
      },
      error: (error) => {
        console.error('Error loading destination types:', error);
        this.showAlert('warning', 'Could not load destination types catalog.');
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
          destinationTypeId: dest.destinationTypeId,
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
    if (!model.name.trim() || !model.description.trim() || !model.countryCode || model.destinationTypeId === null) {
      this.showAlert('warning', 'Please complete all required fields.');
      return;
    }

    const dto = new UpdateDestinationDto();
    dto.name = model.name.trim();
    dto.description = model.description.trim();
    dto.longDescription = model.longDescription.trim() || undefined;
    dto.countryCode = model.countryCode;
    dto.destinationTypeId = model.destinationTypeId;

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
