import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CatalogService } from './catalog.service';
import { ApiClient, CountryDto, CityDto, DestinationTypeDto } from './api-client';

describe('CatalogService', () => {
  let service: CatalogService;
  let httpMock: HttpTestingController;
  let apiClient: jasmine.SpyObj<ApiClient>;

  const mockCountries: CountryDto[] = [
    CountryDto.fromJS({ code: 'ESP', name: 'España', region: 'Europe', isActive: true }),
    CountryDto.fromJS({ code: 'USA', name: 'United States', region: 'North America', isActive: true }),
    CountryDto.fromJS({ code: 'MEX', name: 'México', region: 'North America', isActive: true })
  ];

  const mockCities: CityDto[] = [
    CityDto.fromJS({ id: 1, countryCode: 'ESP', name: 'Barcelona', isActive: true }),
    CityDto.fromJS({ id: 2, countryCode: 'ESP', name: 'Madrid', isActive: true }),
    CityDto.fromJS({ id: 3, countryCode: 'USA', name: 'New York', isActive: true })
  ];

  const mockDestinationTypes: DestinationTypeDto[] = [
    DestinationTypeDto.fromJS({ id: 1, code: 'BEACH', name: 'Beach', icon: 'beach_access', displayOrder: 1, isActive: true }),
    DestinationTypeDto.fromJS({ id: 2, code: 'MOUNTAIN', name: 'Mountain', icon: 'terrain', displayOrder: 2, isActive: true }),
    DestinationTypeDto.fromJS({ id: 3, code: 'CITY', name: 'City', icon: 'location_city', displayOrder: 3, isActive: true })
  ];

  beforeEach(() => {
    // Crear spy object para ApiClient
    const apiClientSpy = jasmine.createSpyObj('ApiClient', [
      'countriesAll',
      'countries',
      'cities',
      'destinationTypesAll',
      'destinationTypes'
    ]);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        CatalogService,
        { provide: ApiClient, useValue: apiClientSpy }
      ]
    });

    service = TestBed.inject(CatalogService);
    httpMock = TestBed.inject(HttpTestingController);
    apiClient = TestBed.inject(ApiClient) as jasmine.SpyObj<ApiClient>;
  });

  afterEach(() => {
    httpMock.verify();
    service.clearCache();
  });

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  describe('getCountries', () => {
    it('debería obtener países del backend', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      service.getCountries().subscribe(countries => {
        expect(countries).toEqual(mockCountries);
        expect(countries.length).toBe(3);
        expect(apiClient.countriesAll).toHaveBeenCalledWith(true);
        done();
      });
    });

    it('debería cachear países y no llamar al backend en segunda petición', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      // Primera llamada
      service.getCountries().subscribe(() => {
        expect(apiClient.countriesAll).toHaveBeenCalledTimes(1);

        // Segunda llamada debería usar caché
        service.getCountries().subscribe(countries => {
          expect(countries).toEqual(mockCountries);
          expect(apiClient.countriesAll).toHaveBeenCalledTimes(1); // No debería llamar nuevamente
          done();
        });
      });
    });

    it('debería recargar datos cuando forceRefresh es true', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      // Primera llamada
      service.getCountries().subscribe(() => {
        expect(apiClient.countriesAll).toHaveBeenCalledTimes(1);

        // Segunda llamada con forceRefresh
        service.getCountries(true, true).subscribe(countries => {
          expect(countries).toEqual(mockCountries);
          expect(apiClient.countriesAll).toHaveBeenCalledTimes(2);
          done();
        });
      });
    });

    it('debería manejar errores y retornar array vacío', (done) => {
      apiClient.countriesAll.and.returnValue(
        Promise.reject(new Error('Network error')) as any
      );

      service.getCountries().subscribe(countries => {
        expect(countries).toEqual([]);
        done();
      });
    });
  });

  describe('getCities', () => {
    it('debería obtener todas las ciudades sin filtro', (done) => {
      apiClient.cities.and.returnValue(
        new Promise<CityDto[]>((resolve) => resolve(mockCities)) as any
      );

      service.getCities().subscribe(cities => {
        expect(cities).toEqual(mockCities);
        expect(cities.length).toBe(3);
        expect(apiClient.cities).toHaveBeenCalledWith(undefined, true);
        done();
      });
    });

    it('debería obtener ciudades filtradas por país', (done) => {
      const espCities = mockCities.filter(c => c.countryCode === 'ESP');
      apiClient.cities.and.returnValue(
        new Promise<CityDto[]>((resolve) => resolve(espCities)) as any
      );

      service.getCities('ESP').subscribe(cities => {
        expect(cities.length).toBe(2);
        expect(cities.every(c => c.countryCode === 'ESP')).toBe(true);
        expect(apiClient.cities).toHaveBeenCalledWith('ESP', true);
        done();
      });
    });

    it('debería cachear ciudades cuando no hay filtro de país', (done) => {
      apiClient.cities.and.returnValue(
        new Promise<CityDto[]>((resolve) => resolve(mockCities)) as any
      );

      service.getCities().subscribe(() => {
        service.getCities().subscribe(cities => {
          expect(apiClient.cities).toHaveBeenCalledTimes(1);
          expect(cities).toEqual(mockCities);
          done();
        });
      });
    });

    it('NO debería cachear cuando hay filtro de país', (done) => {
      const espCities = mockCities.filter(c => c.countryCode === 'ESP');
      apiClient.cities.and.returnValue(
        new Promise<CityDto[]>((resolve) => resolve(espCities)) as any
      );

      service.getCities('ESP').subscribe(() => {
        service.getCities('ESP').subscribe(() => {
          expect(apiClient.cities).toHaveBeenCalledTimes(2);
          done();
        });
      });
    });
  });

  describe('getDestinationTypes', () => {
    it('debería obtener tipos de destino del backend', (done) => {
      apiClient.destinationTypesAll.and.returnValue(
        new Promise<DestinationTypeDto[]>((resolve) => resolve(mockDestinationTypes)) as any
      );

      service.getDestinationTypes().subscribe(types => {
        expect(types).toEqual(mockDestinationTypes);
        expect(types.length).toBe(3);
        expect(apiClient.destinationTypesAll).toHaveBeenCalledWith(true);
        done();
      });
    });

    it('debería cachear tipos y no llamar al backend en segunda petición', (done) => {
      apiClient.destinationTypesAll.and.returnValue(
        new Promise<DestinationTypeDto[]>((resolve) => resolve(mockDestinationTypes)) as any
      );

      service.getDestinationTypes().subscribe(() => {
        service.getDestinationTypes().subscribe(types => {
          expect(types).toEqual(mockDestinationTypes);
          expect(apiClient.destinationTypesAll).toHaveBeenCalledTimes(1);
          done();
        });
      });
    });
  });

  describe('getCountryByCode', () => {
    it('debería retornar país desde caché si existe', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      // Cargar caché primero
      service.getCountries().subscribe(() => {
        service.getCountryByCode('ESP').subscribe(country => {
          expect(country).toEqual(mockCountries[0]);
          expect(apiClient.countries).not.toHaveBeenCalled();
          done();
        });
      });
    });

    it('debería llamar al backend si no está en caché', (done) => {
      const espCountry = mockCountries[0];
      apiClient.countries.and.returnValue(
        new Promise<CountryDto>((resolve) => resolve(espCountry)) as any
      );

      service.getCountryByCode('ESP').subscribe(country => {
        expect(country).toEqual(espCountry);
        expect(apiClient.countries).toHaveBeenCalledWith('ESP');
        done();
      });
    });
  });

  describe('getDestinationTypeById', () => {
    it('debería retornar tipo desde caché si existe', (done) => {
      apiClient.destinationTypesAll.and.returnValue(
        new Promise<DestinationTypeDto[]>((resolve) => resolve(mockDestinationTypes)) as any
      );

      // Cargar caché primero
      service.getDestinationTypes().subscribe(() => {
        service.getDestinationTypeById(1).subscribe(type => {
          expect(type).toEqual(mockDestinationTypes[0]);
          expect(apiClient.destinationTypes).not.toHaveBeenCalled();
          done();
        });
      });
    });

    it('debería llamar al backend si no está en caché', (done) => {
      const beachType = mockDestinationTypes[0];
      apiClient.destinationTypes.and.returnValue(
        new Promise<DestinationTypeDto>((resolve) => resolve(beachType)) as any
      );

      service.getDestinationTypeById(1).subscribe(type => {
        expect(type).toEqual(beachType);
        expect(apiClient.destinationTypes).toHaveBeenCalledWith(1);
        done();
      });
    });
  });

  describe('clearCache', () => {
    it('debería limpiar toda la caché correctamente', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      // Cargar caché
      service.getCountries().subscribe(() => {
        expect(apiClient.countriesAll).toHaveBeenCalledTimes(1);

        // Limpiar caché
        service.clearCache();

        // Siguiente llamada debería ir al backend nuevamente
        service.getCountries().subscribe(() => {
          expect(apiClient.countriesAll).toHaveBeenCalledTimes(2);
          done();
        });
      });
    });
  });

  describe('Helper methods', () => {
    it('getCountryCodes debería retornar array de códigos de país', (done) => {
      apiClient.countriesAll.and.returnValue(
        new Promise<CountryDto[]>((resolve) => resolve(mockCountries)) as any
      );

      service.getCountryCodes().subscribe(codes => {
        expect(codes).toEqual(['ESP', 'USA', 'MEX']);
        done();
      });
    });

    it('getDestinationTypeNames debería retornar array de nombres de tipos', (done) => {
      apiClient.destinationTypesAll.and.returnValue(
        new Promise<DestinationTypeDto[]>((resolve) => resolve(mockDestinationTypes)) as any
      );

      service.getDestinationTypeNames().subscribe(names => {
        expect(names).toEqual(['Beach', 'Mountain', 'City']);
        done();
      });
    });
  });
});
