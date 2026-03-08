import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiClient, CreateDestinationDto, DestinationType } from '../services/api-client';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { COUNTRY_OPTIONS, getCountryNameByCode } from '../shared/enums/country.enum';

@Component({
  selector: 'app-create-destination-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent],
  templateUrl: './create-destination-page.component.html',
  styleUrls: ['./create-destination-page.component.css']
})
export class CreateDestinationPageComponent implements OnInit {
  private readonly apiService = inject(ApiClient);
  private readonly router = inject(Router);

  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  countries = signal<string[]>(COUNTRY_OPTIONS);
  destinationTypes = signal<string[]>([]);
  selectedImageName = signal<string>('');

  formModel: { name: string; countryCode: string; type: number; description: string; longDescription: string } = {
    name: '',
    countryCode: '',
    type: -1,
    description: '',
    longDescription: ''
  };

  ngOnInit(): void {
    this.loadCountries();
    this.loadDestinationTypes();
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

  loadDestinationTypes(): void {
    this.apiService.types().subscribe({
      next: (types: string[]) => this.destinationTypes.set(types || []),
      error: () => this.destinationTypes.set(['Beach', 'Mountain', 'City', 'Cultural', 'Adventure', 'Relax'])
    });
  }

  getCountryDisplayName(countryCode?: string): string {
    return getCountryNameByCode(countryCode) || countryCode || 'Not specified';
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;
    this.selectedImageName.set(file?.name || '');
  }

  onCancel(): void {
    this.router.navigate(['/destinations']);
  }

  onSubmit(): void {
    const model = this.formModel;
    if (!model.name.trim() || !model.description.trim() || !model.countryCode || model.type < 0) {
      this.showAlert('warning', 'Please complete all required fields.');
      return;
    }

    const dto = new CreateDestinationDto();
    dto.name = model.name.trim();
    dto.description = model.description.trim();
    dto.longDescription = model.longDescription.trim() || undefined;
    dto.countryCode = model.countryCode;
    dto.type = this.getDestinationTypeFromIndex(model.type);

    this.loading.set(true);
    this.hideAlert();

    this.apiService.destinationsPOST(dto).subscribe({
      next: (created) => {
        this.loading.set(false);
        this.showAlert('success', `Destination "${created.name}" created successfully.`);
        setTimeout(() => this.router.navigate(['/destinations', created.id]), 700);
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error creating destination: ' + (error?.message || 'Unknown error'));
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

  private getDestinationTypeFromIndex(index: number): DestinationType {
    const enumValues = Object.values(DestinationType).filter(v => typeof v === 'number') as DestinationType[];
    return enumValues[index] || DestinationType._0;
  }
}
