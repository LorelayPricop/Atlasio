import { DestinationType } from '../../services/api-client';

export interface DestinationTypeOption {
  value: DestinationType;
  label: string;
}

export const DESTINATION_TYPE_OPTIONS: DestinationTypeOption[] = [
  { value: DestinationType._0, label: 'Beach' },
  { value: DestinationType._1, label: 'Mountain' },
  { value: DestinationType._2, label: 'City' },
  { value: DestinationType._3, label: 'Cultural' },
  { value: DestinationType._4, label: 'Adventure' },
  { value: DestinationType._5, label: 'Relax' }
];

export function getDestinationTypeLabel(type: DestinationType | number | string | null | undefined): string {
  if (type === null || type === undefined || type === '') {
    return '';
  }

  const numericType = typeof type === 'string' ? Number(type) : Number(type);
  const option = DESTINATION_TYPE_OPTIONS.find(item => Number(item.value) === numericType);
  return option?.label ?? String(type);
}
