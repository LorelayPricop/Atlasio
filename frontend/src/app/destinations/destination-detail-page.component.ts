import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DestinationDto, ApiClient } from '../services/api-client';
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

  destination = signal<DestinationDto | null>(null);
  loading = signal<boolean>(false);
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadDestination(parseInt(id, 10));
    } else {
      this.showAlert('error', 'Invalid destination ID');
    }
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
   * Obtiene el nombre completo del país desde el catálogo o devuelve el código
   */
  getCountryName(countryCode?: string): string {
    if (!countryCode) return 'Not specified';
    // El nombre se cargaría en tiempo real, por ahora devolver el código
    // TODO: implementar caché de CountryDto en detalle si es necesario
    return countryCode;
  }

  getStatusClass(status?: string): string {
    if (!status) {
      return 'status-inactive';
    }

    return status.toLowerCase() === 'active' ? 'status-active' : 'status-inactive';
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
