import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DestinationDto, CreateDestinationDto, UpdateDestinationDto, DestinationType, DestinationDtoPagedResultDto, ApiClient } from '../services/api-client';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { LoadingComponent } from '../shared/loading/loading.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { ModalComponent } from '../shared/modal/modal.component';
import { ConfirmComponent } from '../shared/confirm/confirm.component';
import { DestinationCategory } from '../shared/enums/destination-category.enum';
import { CITY_OPTIONS } from '../shared/enums/city.enum';
import { COUNTRY_OPTIONS, getCountryNameByCode } from '../shared/enums/country.enum';

@Component({
  selector: 'app-destinations-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, LoadingComponent, AlertComponent, ModalComponent, ConfirmComponent],
  templateUrl: './destinations-page.component.html',
  styleUrls: ['./destinations-page.component.css']
})
export class DestinationsPageComponent implements OnInit, OnDestroy {
  private readonly apiService = inject(ApiClient);
  private readonly router = inject(Router);

  // Estado de filtros y datos
  filter = signal<{ page?: number; pageSize?: number; search?: string; countryCode?: string; type?: string }>({ page: 1, pageSize: 5 });
  destinations = signal<DestinationDto[]>([]);
  totalCount = signal<number>(0);
  loading = signal<boolean>(false);
  selectedId = signal<number | null>(null);
  viewMode = signal<'table' | 'grid'>('table');
  sortState = signal<{ key: 'id' | 'name' | 'description' | 'countryCode' | 'type' | 'lastModif'; dir: 'asc' | 'desc' } | null>(null);
  
  // Estados de alertas
  alert = signal<{ show: boolean; type: string; message: string }>({
    show: false,
    type: 'info',
    message: ''
  });

  // Modelos para ngModel
  searchTerm = '';
  countryCode = '';
  type: DestinationType = DestinationType._0;
  pageSize = 5;
  
  // Opciones para selectores
  pageSizeOptions = [5, 10, 20, 50, 100];
  cityOptions = CITY_OPTIONS;

  // Opciones para selects
  destinationTypes = signal<string[]>([]);
  countries = signal<string[]>(COUNTRY_OPTIONS);

  // Estado modal y formulario
  isModalOpen = signal<boolean>(false);
  isEditing = signal<boolean>(false);
  formModel: { id?: number; name: string; description: string; countryCode: string; type: number } =
    { name: '', description: '', countryCode: '', type: 0 };

  // Estado confirmación
  isConfirmOpen = signal<boolean>(false);
  confirmData = signal<{ title: string; message: string; onConfirm: () => void } | null>(null);

  // Búsqueda con debounce
  private searchChange$ = new Subject<string>();
  private subscriptions: Subscription[] = [];

  ngOnInit(): void {
    this.loadCountries();
    this.loadDestinationTypes();
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

  loadCountries(): void {
    this.apiService.countries().subscribe({
      next: (countries: string[]) => {
        const merged = new Set<string>([...COUNTRY_OPTIONS, ...(countries || [])]);
        this.countries.set(Array.from(merged));
      },
      error: (error: any) => {
        console.error('Error al cargar países:', error);
        this.countries.set(COUNTRY_OPTIONS);
      }
    });
  }

  loadDestinationTypes(): void {
    this.apiService.types().subscribe({
      next: (types: string[]) => this.destinationTypes.set(types),
      error: (error: any) => console.error('Error al cargar tipos de destino:', error)
    });
  }

  loadDestinations(): void {
    this.loading.set(true);
    this.hideAlert();
    
    const currentFilter = this.filter();
    this.apiService.destinationsGET(
      currentFilter.search,
      currentFilter.countryCode,
      currentFilter.type as unknown as DestinationType,
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
        this.showAlert('error', 'Error al cargar los destinos: ' + error.message);
      }
    });
  }

  onSearch(): void {
    this.filter.update(f => ({ 
      ...f, 
      search: this.searchTerm || undefined, 
      page: 1 
    }));
    this.loadDestinations();
  }

  onSearchChange(value: string): void {
    this.searchTerm = value;
    this.searchChange$.next(value);
  }

  onFilterChange(): void {
    this.filter.update(f => ({
      ...f,
      page: 1,
      countryCode: this.countryCode || undefined,
      type: this.type ? String(this.type) : undefined,
      pageSize: this.pageSize
    }));
    this.loadDestinations();
  }

  onPageSizeChange(): void {
    this.filter.update(f => ({
      ...f,
      page: 1,
      pageSize: this.pageSize
    }));
    this.loadDestinations();
  }

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
    const totalPages = Math.ceil(this.totalCount() / (this.filter().pageSize || 5));
    return currentPage < totalPages;
  }

  getStartResult(): number {
    if (this.totalCount() === 0) return 0;
    const currentPage = this.filter().page || 1;
    const pageSize = this.filter().pageSize || 5;
    return (currentPage - 1) * pageSize + 1;
  }

  getEndResult(): number {
    const currentPage = this.filter().page || 1;
    const pageSize = this.filter().pageSize || 5;
    const lastOnPage = (currentPage - 1) * pageSize + this.getSortedDestinations().length;
    return Math.min(lastOnPage, this.totalCount());
  }

  selectRow(id: number): void {
    this.selectedId.set(id === this.selectedId() ? null : id);
  }

  setViewMode(mode: 'table' | 'grid'): void {
    this.viewMode.set(mode);
  }

  onSort(key: 'id' | 'name' | 'description' | 'countryCode' | 'type' | 'lastModif'): void {
    const current = this.sortState();
    if (current?.key === key) {
      const nextDir = current.dir === 'asc' ? 'desc' : 'asc';
      this.sortState.set({ key, dir: nextDir });
    } else {
      this.sortState.set({ key, dir: 'asc' });
    }
  }

  getSortedDestinations(): DestinationDto[] {
    const data = [...(this.destinations() || [])];
    const sort = this.sortState();
    if (!sort) return data;

    const compare = (a: any, b: any): number => {
      const av = a ?? '';
      const bv = b ?? '';
      if (typeof av === 'number' && typeof bv === 'number') return av - bv;
      const as = av instanceof Date ? av.getTime() : String(av).toLowerCase();
      const bs = bv instanceof Date ? bv.getTime() : String(bv).toLowerCase();
      if (as < bs) return -1;
      if (as > bs) return 1;
      return 0;
    };

    data.sort((a, b) => {
      let res = 0;
      switch (sort.key) {
        case 'id': res = compare(a.id, b.id); break;
        case 'name': res = compare(a.name, b.name); break;
        case 'description': res = compare(a.description, b.description); break;
        case 'countryCode': res = compare(a.countryCode, b.countryCode); break;
        case 'type': res = compare(a.type, b.type); break;
        case 'lastModif': res = compare(a.lastModif, b.lastModif); break;
      }
      return sort.dir === 'asc' ? res : -res;
    });
    return data;
  }

  onCreate(): void {
    this.router.navigate(['/destinations/new']);
  }

  onEdit(): void {
    if (this.selectedId() == null) {
      this.showAlert('warning', 'Selecciona un destino para editar');
      return;
    }
    this.router.navigate(['/destinations', this.selectedId(), 'edit']);
  }

  onModalClose(): void {
    this.isModalOpen.set(false);
  }

  onSubmitForm(): void {
    const model = this.formModel;
    if (!model.name?.trim() || !model.description?.trim() || !model.countryCode) {
      this.showAlert('warning', 'Completa los campos obligatorios');
      return;
    }
    this.loading.set(true);
    
    // Convertir el índice del formulario al enum correspondiente
    const typeEnum = this.getDestinationTypeFromIndex(model.type);
    
    if (this.isEditing()) {
      const dto = new UpdateDestinationDto();
      dto.name = model.name.trim();
      dto.description = model.description.trim();
      dto.countryCode = model.countryCode;
      dto.type = typeEnum;
      this.apiService.destinationsPUT(model.id!, dto).subscribe({
        next: (updated: DestinationDto) => {
          this.loading.set(false);
          this.isModalOpen.set(false);
          this.showAlert('success', `Destino "${updated.name}" actualizado exitosamente`);
          this.loadDestinations();
        },
        error: (error: any) => {
          this.loading.set(false);
          this.showAlert('error', 'Error al actualizar el destino: ' + this.extractApiErrorDetails(error));
        }
      });
    } else {
      const dto = new CreateDestinationDto();
      dto.name = model.name.trim();
      dto.description = model.description.trim();
      dto.countryCode = model.countryCode;
      dto.type = typeEnum;
      this.apiService.destinationsPOST(dto).subscribe({
        next: (created: DestinationDto) => {
          this.loading.set(false);
          this.isModalOpen.set(false);
          this.showAlert('success', `Destino "${created.name}" creado exitosamente`);
          this.loadDestinations();
        },
        error: (error: any) => {
          this.loading.set(false);
          this.showAlert('error', 'Error al crear el destino: ' + this.extractApiErrorDetails(error));
        }
      });
    }
  }

  private extractApiErrorDetails(error: any): string {
    try {
      const result = error?.result ?? null;
      if (result?.errors) {
        const parts: string[] = [];
        for (const key of Object.keys(result.errors)) {
          const msgs = result.errors[key];
          if (Array.isArray(msgs)) {
            for (const m of msgs) parts.push(`${key}: ${m}`);
          }
        }
        if (parts.length) return parts.join(' | ');
      }
      if (result?.title) return result.title;
      if (result?.detail) return result.detail;
      return error?.message ?? 'Error desconocido';
    } catch {
      return error?.message ?? 'Error desconocido';
    }
  }

  onRemove(): void {
    const id = this.selectedId();
    if (id == null) {
      this.showAlert('warning', 'Selecciona un destino para eliminar');
      return;
    }
    
    const destination = this.destinations().find(d => d.id === id);
    const name = destination?.name || `ID ${id}`;
    
    this.confirmData.set({
      title: 'Eliminar destino',
      message: `¿Estás seguro de que quieres eliminar "${name}"? Esta acción no se puede deshacer.`,
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
        this.showAlert('success', 'Destino eliminado correctamente');
      },
      error: (error: any) => {
        this.loading.set(false);
        this.showAlert('error', 'Error al eliminar el destino: ' + error.message);
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

  getTypeLabel(type: string | number | DestinationType): string {
    const typeStrings = this.destinationTypes();
    
    // Si es un número (índice del enum), obtener el string correspondiente
    if (typeof type === 'number') {
      return typeStrings[type] || String(type);
    }
    
    // Si es un string, devolverlo tal como está
    if (typeof type === 'string') {
      return type;
    }
    
    // Si es un enum DestinationType, convertir a número y luego a string
    if (type !== null && type !== undefined) {
      const enumIndex = Number(type);
      if (!isNaN(enumIndex) && enumIndex >= 0 && enumIndex < typeStrings.length) {
        return typeStrings[enumIndex] || String(type);
      }
    }
    
    return String(type || '');
  }

  getTypeClass(type: string | number | DestinationType): string {
    return this.getTypeCategory(type);
  }

  getCountryDisplayName(countryCode?: string): string {
    return getCountryNameByCode(countryCode) || countryCode || 'Not specified';
  }

  getTypeIcon(type: string | number | DestinationType): string {
    const category = this.getTypeCategory(type);
    if (category === DestinationCategory.Beach) return 'beach_access';
    if (category === DestinationCategory.Mountain) return 'terrain';
    if (category === DestinationCategory.Cultural) return 'museum';
    if (category === DestinationCategory.Adventure) return 'hiking';
    if (category === DestinationCategory.Relax) return 'self_improvement';
    return 'location_city';
  }

  private getTypeCategory(type: string | number | DestinationType): DestinationCategory {
    const normalized = this.normalizeTypeLabel(this.getTypeLabel(type));

    if (normalized.includes('beach') || normalized.includes('playa')) return DestinationCategory.Beach;
    if (normalized.includes('mountain') || normalized.includes('montana')) return DestinationCategory.Mountain;
    if (normalized.includes('cultural') || normalized.includes('cultura') || normalized.includes('historic') || normalized.includes('historico')) return DestinationCategory.Cultural;
    if (normalized.includes('adventure') || normalized.includes('aventura')) return DestinationCategory.Adventure;
    if (normalized.includes('relax') || normalized.includes('relaj')) return DestinationCategory.Relax;
    return DestinationCategory.City;
  }

  private normalizeTypeLabel(value: string): string {
    return (value || '')
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .trim();
  }

  private getDestinationTypeFromIndex(index: number): DestinationType {
    // Mapear el índice del formulario al enum correspondiente
    const enumValues = Object.values(DestinationType).filter(v => typeof v === 'number') as DestinationType[];
    return enumValues[index] || DestinationType._0;
  }

  private getIndexFromDestinationType(type: DestinationType): number {
    // Mapear el enum al índice del formulario
    const enumValues = Object.values(DestinationType).filter(v => typeof v === 'number') as DestinationType[];
    return enumValues.indexOf(type);
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


