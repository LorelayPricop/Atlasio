import { CountryCode, getCountryNameByCode } from './country.enum';

export enum City {
  Bali = 'Bali',
  Paris = 'Paris',
  Barcelona = 'Barcelona',
  Madrid = 'Madrid',
  Rome = 'Rome',
  Lisbon = 'Lisbon',
  London = 'London',
  Berlin = 'Berlin',
  NewYork = 'New York',
  LosAngeles = 'Los Angeles',
  Tokyo = 'Tokyo',
  Sydney = 'Sydney',
  BuenosAires = 'Buenos Aires'
}

export const CITY_OPTIONS: string[] = Object.values(City);

const CITY_COUNTRY_CODE: Partial<Record<City, CountryCode>> = {
  [City.Bali]: CountryCode.IDN,
  [City.Paris]: CountryCode.FRA,
  [City.Barcelona]: CountryCode.ESP,
  [City.Madrid]: CountryCode.ESP,
  [City.Rome]: CountryCode.ITA,
  [City.Lisbon]: CountryCode.POR,
  [City.London]: CountryCode.GBR,
  [City.Berlin]: CountryCode.ALE,
  [City.NewYork]: CountryCode.USA,
  [City.LosAngeles]: CountryCode.USA,
  [City.Tokyo]: CountryCode.JPN,
  [City.Sydney]: CountryCode.AUS,
  [City.BuenosAires]: CountryCode.ARG
};

export function getCountryNameByCity(cityName?: string): string | undefined {
  if (!cityName) {
    return undefined;
  }

  const found = CITY_OPTIONS.find(city => city.toLowerCase() === cityName.trim().toLowerCase());
  if (!found) {
    return undefined;
  }

  const countryCode = CITY_COUNTRY_CODE[found as City];
  return getCountryNameByCode(countryCode);
}
