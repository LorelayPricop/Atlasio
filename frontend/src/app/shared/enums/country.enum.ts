export enum CountryCode {
  ESP = 'ESP',
  FRA = 'FRA',
  ITA = 'ITA',
  POR = 'POR',
  GBR = 'GBR',
  ALE = 'ALE',
  USA = 'USA',
  MEX = 'MEX',
  GRC = 'GRC',
  PER = 'PER',
  BRA = 'BRA',
  CHE = 'CHE',
  ARG = 'ARG',
  JPN = 'JPN',
  IDN = 'IDN',
  CHN = 'CHN',
  IND = 'IND',
  AUS = 'AUS',
  NZL = 'NZL'
}

export const COUNTRY_OPTIONS: string[] = Object.values(CountryCode);

const COUNTRY_NAME_BY_CODE: Record<CountryCode, string> = {
  [CountryCode.ESP]: 'Spain',
  [CountryCode.FRA]: 'France',
  [CountryCode.ITA]: 'Italy',
  [CountryCode.POR]: 'Portugal',
  [CountryCode.GBR]: 'United Kingdom',
  [CountryCode.ALE]: 'Germany',
  [CountryCode.USA]: 'United States',
  [CountryCode.MEX]: 'Mexico',
  [CountryCode.GRC]: 'Greece',
  [CountryCode.PER]: 'Peru',
  [CountryCode.BRA]: 'Brazil',
  [CountryCode.CHE]: 'Switzerland',
  [CountryCode.ARG]: 'Argentina',
  [CountryCode.JPN]: 'Japan',
  [CountryCode.IDN]: 'Indonesia',
  [CountryCode.CHN]: 'China',
  [CountryCode.IND]: 'India',
  [CountryCode.AUS]: 'Australia',
  [CountryCode.NZL]: 'New Zealand'
};

export function getCountryNameByCode(countryCode?: string): string | undefined {
  if (!countryCode) {
    return undefined;
  }

  const normalizedCode = countryCode.trim().toUpperCase() as CountryCode;
  return COUNTRY_NAME_BY_CODE[normalizedCode];
}
