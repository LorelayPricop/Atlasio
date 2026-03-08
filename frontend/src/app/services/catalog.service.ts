import { Injectable, inject, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, shareReplay, tap, catchError } from 'rxjs/operators';
import { ApiClient, CountryDto, CityDto, DestinationTypeDto } from './api-client';

/**
 * Servicio centralizado para gestión de catálogos del backend
 * Implementa caché en memoria para evitar llamadas HTTP redundantes
 */
@Injectable({
  providedIn: 'root'
})
export class CatalogService {
  private readonly apiClient = inject(ApiClient);

  // Señales para estado de catálogos cargados
  private countriesLoaded = signal<boolean>(false);
  private citiesLoaded = signal<boolean>(false);
  private destinationTypesLoaded = signal<boolean>(false);

  // Caché de observables compartidos para evitar llamadas duplicadas
  private countries$: Observable<CountryDto[]> | null = null;
  private cities$: Observable<CityDto[]> | null = null;
  private destinationTypes$: Observable<DestinationTypeDto[]> | null = null;

  // Caché de datos en memoria
  private countriesCache: CountryDto[] = [];
  private citiesCache: CityDto[] = [];
  private destinationTypesCache: DestinationTypeDto[] = [];

  /**
   * Obtiene el catálogo de países con caché
   * @param onlyActive Filtrar solo países activos (default: true)
   * @param forceRefresh Forzar recarga desde backend
   */
  getCountries(onlyActive: boolean = true, forceRefresh: boolean = false): Observable<CountryDto[]> {
    if (!forceRefresh && this.countriesLoaded() && this.countriesCache.length > 0) {
      return of(this.countriesCache);
    }

    if (!forceRefresh && this.countries$) {
      return this.countries$;
    }

    this.countries$ = this.apiClient.countriesAll(onlyActive).pipe(
      tap(countries => {
        this.countriesCache = countries;
        this.countriesLoaded.set(true);
      }),
      catchError(error => {
        console.error('Error loading countries catalog:', error);
        return of([]);
      }),
      shareReplay(1)
    );

    return this.countries$;
  }

  /**
   * Obtiene el catálogo de ciudades con caché
   * @param countryCode Código de país ISO 3166-1 alpha-3 (opcional)
   * @param onlyActive Filtrar solo ciudades activas (default: true)
   * @param forceRefresh Forzar recarga desde backend
   */
  getCities(countryCode?: string, onlyActive: boolean = true, forceRefresh: boolean = false): Observable<CityDto[]> {
    // Si hay countryCode, no usar caché global (filtro específico)
    if (countryCode) {
      return this.apiClient.cities(countryCode, onlyActive).pipe(
        catchError(error => {
          console.error(`Error loading cities for country ${countryCode}:`, error);
          return of([]);
        })
      );
    }

    // Caché solo para listado completo sin filtro
    if (!forceRefresh && this.citiesLoaded() && this.citiesCache.length > 0) {
      return of(this.citiesCache);
    }

    if (!forceRefresh && this.cities$) {
      return this.cities$;
    }

    this.cities$ = this.apiClient.cities(undefined, onlyActive).pipe(
      tap(cities => {
        this.citiesCache = cities;
        this.citiesLoaded.set(true);
      }),
      catchError(error => {
        console.error('Error loading cities catalog:', error);
        return of([]);
      }),
      shareReplay(1)
    );

    return this.cities$;
  }

  /**
   * Obtiene el catálogo de tipos de destino con caché
   * @param onlyActive Filtrar solo tipos activos (default: true)
   * @param forceRefresh Forzar recarga desde backend
   */
  getDestinationTypes(onlyActive: boolean = true, forceRefresh: boolean = false): Observable<DestinationTypeDto[]> {
    if (!forceRefresh && this.destinationTypesLoaded() && this.destinationTypesCache.length > 0) {
      return of(this.destinationTypesCache);
    }

    if (!forceRefresh && this.destinationTypes$) {
      return this.destinationTypes$;
    }

    this.destinationTypes$ = this.apiClient.destinationTypesAll(onlyActive).pipe(
      tap(types => {
        this.destinationTypesCache = types;
        this.destinationTypesLoaded.set(true);
      }),
      catchError(error => {
        console.error('Error loading destination types catalog:', error);
        return of([]);
      }),
      shareReplay(1)
    );

    return this.destinationTypes$;
  }

  /**
   * Obtiene un país específico por código
   * Primero intenta buscarlo en caché, luego llama al backend si es necesario
   */
  getCountryByCode(code: string): Observable<CountryDto | undefined> {
    if (this.countriesLoaded() && this.countriesCache.length > 0) {
      const country = this.countriesCache.find(c => c.code === code);
      if (country) {
        return of(country);
      }
    }

    // Si no está en caché, llamar endpoint específico
    return this.apiClient.countries(code).pipe(
      catchError(error => {
        console.error(`Error loading country ${code}:`, error);
        return of(undefined);
      })
    );
  }

  /**
   * Obtiene un tipo de destino específico por ID
   * Primero intenta buscarlo en caché, luego llama al backend si es necesario
   */
  getDestinationTypeById(id: number): Observable<DestinationTypeDto | undefined> {
    if (this.destinationTypesLoaded() && this.destinationTypesCache.length > 0) {
      const type = this.destinationTypesCache.find(t => t.id === id);
      if (type) {
        return of(type);
      }
    }

    // Si no está en caché, llamar endpoint específico
    return this.apiClient.destinationTypes(id).pipe(
      catchError(error => {
        console.error(`Error loading destination type ${id}:`, error);
        return of(undefined);
      })
    );
  }

  /**
   * Limpia toda la caché forzando recarga en próxima petición
   */
  clearCache(): void {
    this.countries$ = null;
    this.cities$ = null;
    this.destinationTypes$ = null;
    this.countriesCache = [];
    this.citiesCache = [];
    this.destinationTypesCache = [];
    this.countriesLoaded.set(false);
    this.citiesLoaded.set(false);
    this.destinationTypesLoaded.set(false);
  }

  /**
   * Obtiene códigos de países activos (helper para retrocompatibilidad)
   */
  getCountryCodes(): Observable<string[]> {
    return this.getCountries().pipe(
      map(countries => countries.map(c => c.code || '').filter(code => code.length > 0))
    );
  }

  /**
   * Obtiene nombres de display de tipos de destino (helper para retrocompatibilidad)
   */
  getDestinationTypeNames(): Observable<string[]> {
    return this.getDestinationTypes().pipe(
      map(types => types.map(t => t.name || 'Unknown'))
    );
  }
}
