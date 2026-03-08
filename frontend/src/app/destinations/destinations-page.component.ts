import { ChangeDetectionStrategy, Component, OnInit, OnDestroy, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DestinationDto, DestinationDtoPagedResultDto, ApiClient, CountryDto, DestinationTypeDto } from '../services/api-client';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { ConfirmComponent } from '../shared/confirm/confirm.component';
import { CatalogService } from '../services/catalog.service';

@Component({
  selector: 'app-destinations-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent, ConfirmComponent],
  templateUrl: './destinations-page.component.html',
  styleUrls: ['./destinations-page.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DestinationsPageComponent implements OnInit, OnDestroy {
  private readonly apiService = inject(ApiClient);
  private readonly catalogService = inject(CatalogService);
  private readonly router = inject(Router);

  // Estado de filtros y datos
  filter = signal<{ 
    page?: number; 
    pageSize?: number; 
    search?: string; 
    countryCode?: string; 
    destinationTypeId?: number;
  }>({ page: 1, pageSize: 10 });
  
  destinations = signal<DestinationDto[]>([]);
  totalCount = signal<number>(0);
  loading = signal<boolean>(false);
  selectedId = signal<number | null>(null);
  viewMode = signal<'table' | 'grid'>('table');
  
  // Catálogos cargados del backend
  countries = signal<CountryDto[]>([]);
  destinationTypes = signal<DestinationTypeDto[]>([]);
  readonly defaultDestinationImage = 'https://images.unsplash.com/photo-1476514525535-07fb3b4ae5f1?w=400&h=300&fit=crop';

  private readonly countriesMap = computed(() => {
    const map = new Map<string, string>();
    for (const country of this.countries()) {
      if (country.code) {
        map.set(country.code, country.name || country.code);
      }
    }
    return map;
  });

  private readonly destinationTypesMap = computed(() => {
    const map = new Map<number, DestinationTypeDto>();
    for (const type of this.destinationTypes()) {
      if (type.id != null) {
        map.set(type.id, type);
      }
    }
    return map;
  });
  
  // Estados de alertas
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  // Modelos para ngModel de filtros
  searchTerm = '';
  countryCode = '';
  destinationTypeId: number | null = null;
  pageSize = 10;
  
  // Opciones para paginación
  pageSizeOptions = [5, 10, 20, 50, 100];

  // Estado confirmación
  isConfirmOpen = signal<boolean>(false);
  confirmData = signal<{ title: string; message: string; onConfirm: () => void } | null>(null);

  // Búsqueda con debounce
  private searchChange$ = new Subject<string>();
  private subscriptions: Subscription[] = [];

  ngOnInit(): void {
    this.loadCatalogs();
    this.loadDestinations();

    // Debounce de la búsqueda
    const sub = this.searchChange$
      .pipe(debounceTime(700), distinctUntilChanged())
      .subscribe(term => {
        this.filter.update(f => ({ ...f, search: term || undefined, page: 1 }));
        this.loadDestinations();
      });
    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    for (const s of this.subscriptions) {
      try { s.unsubscribe(); } catch {}
    }
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
      error: (error: any) => {
        console.error('Error loading countries catalog:', error);
      }
    });

    // Cargar tipos de destino
    this.catalogService.getDestinationTypes().subscribe({
      next: (types) => {
        this.destinationTypes.set(types);
      },
      error: (error: any) => {
        console.error('Error loading destination types catalog:', error);
      }
    });
  }

  /**
   * Carga destinos desde el backend con filtros y paginación
   */
  loadDestinations(): void {
    this.loading.set(true);
    this.hideAlert();
    
    const currentFilter = this.filter();
    this.apiService.destinationsGET(
      currentFilter.search,
      currentFilter.countryCode,
      currentFilter.destinationTypeId,
      currentFilter.page,
      currentFilter.pageSize
    ).subscribe({
      next: (response: DestinationDtoPagedResultDto) => {
        this.destinations.set(response.items || []);
        this.totalCount.set(response.totalCount || 0);
        this.loading.set(false);
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error loading destinations: ' + (error.message || 'Unknown error'));
      }
    });
  }

  /**
   * Maneja cambio en el campo de búsqueda con debounce
   */
  onSearchChange(value: string): void {
    this.searchTerm = value;
    this.searchChange$.next(value);
  }

  /**
   * Maneja cambios en filtros (país y tipo)
   */
  onFilterChange(): void {
    this.filter.update(f => ({
      ...f,
      page: 1,
      countryCode: this.countryCode || undefined,
      destinationTypeId: this.destinationTypeId || undefined,
      pageSize: this.pageSize
    }));
    this.loadDestinations();
  }

  /**
   * Maneja cambio en tamaño de página
   */
  onPageSizeChange(): void {
    this.filter.update(f => ({
      ...f,
      page: 1,
      pageSize: this.pageSize
    }));
    this.loadDestinations();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.countryCode = '';
    this.destinationTypeId = null;
    this.filter.set({ page: 1, pageSize: this.pageSize });
    this.loadDestinations();
  }

  /**
   * Cambia de página (offset: -1 para anterior, +1 para siguiente)
   */
  changePage(offset: number): void {
    const currentPage = this.filter().page || 1;
    const newPage = Math.max(1, currentPage + offset);
    this.filter.update(f => ({ ...f, page: newPage }));
    this.loadDestinations();
  }

  canGoToPreviousPage(): boolean {
    return (this.filter().page || 1) > 1;
  }

  canGoToNextPage(): boolean {
    const currentPage = this.filter().page || 1;
    const totalPages = Math.ceil(this.totalCount() / (this.filter().pageSize || 10));
    return currentPage < totalPages;
  }

  getStartResult(): number {
    if (this.totalCount() === 0) return 0;
    const currentPage = this.filter().page || 1;
    const pageSize = this.filter().pageSize || 10;
    return (currentPage - 1) * pageSize + 1;
  }

  getEndResult(): number {
    const currentPage = this.filter().page || 1;
    const pageSize = this.filter().pageSize || 10;
    const lastOnPage = (currentPage - 1) * pageSize + this.destinations().length;
    return Math.min(lastOnPage, this.totalCount());
  }

  selectRow(id: number): void {
    this.selectedId.set(id === this.selectedId() ? null : id);
  }

  setViewMode(mode: 'table' | 'grid'): void {
    this.viewMode.set(mode);
  }

  onCreate(): void {
    this.router.navigate(['/destinations/new']);
  }

  onEdit(): void {
    if (this.selectedId() == null) {
      this.showAlert('warning', 'Please select a destination to edit');
      return;
    }
    this.router.navigate(['/destinations', this.selectedId(), 'edit']);
  }

  onView(): void {
    if (this.selectedId() == null) {
      this.showAlert('warning', 'Please select a destination to view');
      return;
    }
    this.router.navigate(['/destinations', this.selectedId()]);
  }

  onDelete(): void {
    const id = this.selectedId();
    if (id == null) {
      this.showAlert('warning', 'Please select a destination to delete');
      return;
    }
    const destination = this.destinations().find(d => d.id === id);
    this.confirmData.set({
      title: 'Delete Destination',
      message: `Are you sure you want to delete "${destination?.name || 'this destination'}"?`,
      onConfirm: () => this.executeDelete(id)
    });
    this.isConfirmOpen.set(true);
  }

  private executeDelete(id: number): void {
    this.loading.set(true);
    this.apiService.destinationsDELETE(id).subscribe({
      next: () => {
        this.selectedId.set(null);
        this.loadDestinations();
        this.showAlert('success', 'Destination deleted successfully');
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error deleting destination: ' + (error.message || 'Unknown error'));
      }
    });
  }

  onConfirmClose(): void {
    this.isConfirmOpen.set(false);
    this.confirmData.set(null);
  }

  onConfirmAction(): void {
    const data = this.confirmData();
    if (data?.onConfirm) {
      data.onConfirm();
    }
    this.onConfirmClose();
  }

  /**
   * Obtiene el nombre del país desde el catálogo
   */
  getCountryDisplayName(countryCode?: string): string {
    if (!countryCode) return 'Not specified';
    return this.countriesMap().get(countryCode) || countryCode;
  }

  /**
   * Obtiene el icono según el código del tipo de destino
   */
  getTypeIcon(typeCode?: string): string {
    if (!typeCode) return 'location_city';
    const code = typeCode.toLowerCase();
    
    if (code.includes('beach') || code.includes('playa')) return 'beach_access';
    if (code.includes('mountain') || code.includes('montaña')) return 'terrain';
    if (code.includes('cultural') || code.includes('cultura')) return 'museum';
    if (code.includes('adventure') || code.includes('aventura')) return 'hiking';
    if (code.includes('relax')) return 'self_improvement';
    return 'location_city';
  }

  /**
   * Obtiene los colores del tipo de destino desde el catálogo backend
   */
  getTypeColors(destinationTypeId?: number): { background: string; foreground: string } {
    if (!destinationTypeId) {
      return { background: '#f1f5f9', foreground: '#64748b' };
    }

    const type = this.destinationTypesMap().get(destinationTypeId);
    if (!type) {
      return { background: '#f1f5f9', foreground: '#64748b' };
    }

    return {
      background: type.colorBackground || '#eaf0ff',
      foreground: type.colorForeground || '#1d4ed8'
    };
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
