import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DestinationDto, ApiClient, CountryDto, DestinationTypeDto } from '../services/api-client';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { CatalogService } from '../services/catalog.service';

@Component({
  selector: 'app-destination-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule, LoadingComponent, AlertComponent],
  templateUrl: './destination-detail-page.component.html',
  styleUrls: ['./destination-detail-page.component.css']
})
export class DestinationDetailPageComponent implements OnInit {
  private readonly apiService = inject(ApiClient);
  private readonly catalogService = inject(CatalogService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly sanitizer = inject(DomSanitizer);

  destination = signal<DestinationDto | null>(null);
  countries = signal<CountryDto[]>([]);
  destinationTypes = signal<DestinationTypeDto[]>([]);
  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  ngOnInit(): void {
    this.loadCatalogs();
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadDestination(parseInt(id, 10));
    } else {
      this.showAlert('error', 'Invalid destination ID');
    }
  }

  /**
   * Carga catálogos de países y tipos de destino
   */
  loadCatalogs(): void {
    this.catalogService.getCountries().subscribe({
      next: (countries) => this.countries.set(countries),
      error: (error) => console.error('Error loading countries:', error)
    });

    this.catalogService.getDestinationTypes().subscribe({
      next: (types) => this.destinationTypes.set(types),
      error: (error) => console.error('Error loading types:', error)
    });
  }

  loadDestination(id: number): void {
    this.loading.set(true);
    this.hideAlert();

    this.apiService.destinationsGET2(id).subscribe({
      next: (destination: DestinationDto) => {
        this.destination.set(destination);
        this.loading.set(false);
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error loading destination: ' + error.message);
      }
    });
  }

  /**
   * Obtiene el nombre completo del país desde el catálogo
   */
  getCountryName(countryCode?: string): string {
    if (!countryCode) return 'Not specified';
    const country = this.countries().find(c => c.code === countryCode);
    return country?.name || countryCode;
  }

  /**
   * Obtiene el nombre del tipo de destino desde el catálogo
   */
  getTypeName(destinationTypeId?: number): string {
    if (!destinationTypeId) return 'Unknown';
    const type = this.destinationTypes().find(t => t.id === destinationTypeId);
    return type?.name || 'Unknown';
  }

  getStatusClass(status?: string): string {
    if (!status) {
      return 'status-inactive';
    }

    return status.toLowerCase() === 'active' ? 'status-active' : 'status-inactive';
  }

  getMapEmbedUrl(destination: DestinationDto): SafeResourceUrl {
    const countryName = this.getCountryName(destination.countryCode);
    const query = encodeURIComponent(`${destination.name || ''}, ${countryName}`);
    return this.sanitizer.bypassSecurityTrustResourceUrl(`https://maps.google.com/maps?q=${query}&z=6&output=embed`);
  }

  isActiveStatus(status?: string): boolean {
    return (status ?? '').toLowerCase() === 'active';
  }

  onBack(): void {
    this.router.navigate(['/destinations']);
  }

  onEdit(): void {
    const destination = this.destination();
    if (destination?.id) {
      this.router.navigate(['/destinations', destination.id, 'edit']);
    }
  }

  onDelete(): void {
    const destination = this.destination();
    if (destination?.id && confirm(`Are you sure you want to delete "${destination.name}"?`)) {
      this.loading.set(true);
      this.apiService.destinationsDELETE(destination.id).subscribe({
        next: () => {
          this.showAlert('success', 'Destination deleted successfully');
          setTimeout(() => this.router.navigate(['/destinations']), 1500);
        },
        error: (error: any) => {
          this.loading.set(false);
          this.showAlert('error', 'Error deleting destination: ' + error.message);
        }
      });
    }
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
