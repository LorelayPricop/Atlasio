import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiClient, CreateDestinationDto, CountryDto, DestinationTypeDto } from '../services/api-client';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { CatalogService } from '../services/catalog.service';

@Component({
  selector: 'app-create-destination-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent],
  templateUrl: './create-destination-page.component.html',
  styleUrls: ['./create-destination-page.component.css']
})
export class CreateDestinationPageComponent implements OnInit {
  private readonly apiService = inject(ApiClient);
  private readonly catalogService = inject(CatalogService);
  private readonly router = inject(Router);

  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  // Catálogos cargados del backend
  countries = signal<CountryDto[]>([]);
  destinationTypes = signal<DestinationTypeDto[]>([]);
  selectedImageName = signal<string>('');
  selectedImageDataUrl = signal<string>('');

  formModel: { 
    name: string; 
    countryCode: string; 
    destinationTypeId: number | null; 
    description: string; 
    longDescription: string 
  } = {
    name: '',
    countryCode: '',
    destinationTypeId: null,
    description: '',
    longDescription: ''
  };

  ngOnInit(): void {
    this.loadCatalogs();
  }

  /**
   * Carga catálogos de países y tipos de destino desde el backend
   */
  loadCatalogs(): void {
    this.loading.set(true);

    // Cargar países
    this.catalogService.getCountries().subscribe({
      next: (countries) => {
        this.countries.set(countries);
      },
      error: (error) => {
        console.error('Error loading countries:', error);
        this.showAlert('warning', 'Could not load countries catalog. Please refresh the page.');
      }
    });

    // Cargar tipos de destino
    this.catalogService.getDestinationTypes().subscribe({
      next: (types) => {
        this.destinationTypes.set(types);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading destination types:', error);
        this.showAlert('warning', 'Could not load destination types catalog. Please refresh the page.');
        this.loading.set(false);
      }
    });
  }

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files.length > 0 ? input.files[0] : null;

    if (!file) {
      this.selectedImageName.set('');
      this.selectedImageDataUrl.set('');
      return;
    }

    const maxSizeBytes = 10 * 1024 * 1024;
    if (file.size > maxSizeBytes) {
      this.selectedImageName.set('');
      this.selectedImageDataUrl.set('');
      this.showAlert('warning', 'Image exceeds 10MB limit. Please choose a smaller file.');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      const result = typeof reader.result === 'string' ? reader.result : '';
      this.selectedImageName.set(file.name);
      this.selectedImageDataUrl.set(result);
    };
    reader.onerror = () => {
      this.selectedImageName.set('');
      this.selectedImageDataUrl.set('');
      this.showAlert('error', 'Error reading selected image.');
    };

    reader.readAsDataURL(file);
  }

  onCancel(): void {
    this.router.navigate(['/destinations']);
  }

  onSubmit(): void {
    const model = this.formModel;
    if (!model.name.trim() || !model.description.trim() || !model.countryCode || model.destinationTypeId === null) {
      this.showAlert('warning', 'Please complete all required fields.');
      return;
    }

    const dto = new CreateDestinationDto();
    dto.name = model.name.trim();
    dto.description = model.description.trim();
    dto.longDescription = model.longDescription.trim() || undefined;
    dto.imageUrl = this.selectedImageDataUrl() || undefined;
    dto.countryCode = model.countryCode;
    dto.destinationTypeId = model.destinationTypeId;

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
}
