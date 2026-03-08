import { Component } from '@angular/core';
import { DestinationCategory } from '../shared/enums/destination-category.enum';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent {
  readonly destinationCategory = DestinationCategory;

  readonly destinationByTypeData: Array<{ category: DestinationCategory; valuePercent: number }> = [
    { category: DestinationCategory.City, valuePercent: 90 },
    { category: DestinationCategory.Beach, valuePercent: 74 },
    { category: DestinationCategory.Mountain, valuePercent: 56 },
    { category: DestinationCategory.Cultural, valuePercent: 68 },
    { category: DestinationCategory.Adventure, valuePercent: 49 },
    { category: DestinationCategory.Relax, valuePercent: 62 }
  ];

  getCategoryLabel(category: DestinationCategory): string {
    const value = String(category || '');
    return value.charAt(0).toUpperCase() + value.slice(1);
  }
}
