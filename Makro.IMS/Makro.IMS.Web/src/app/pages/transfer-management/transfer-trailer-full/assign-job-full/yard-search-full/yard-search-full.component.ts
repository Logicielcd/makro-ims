import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Door } from '@core/models/master/door.model';
import { Yard } from '@core/models/master/yard.model';
import { DoorService } from '@core/services/master/door.service';
import { YardService } from '@core/services/master/yard.service';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-yard-search-full',
  templateUrl: './yard-search-full.component.html',
})
export class YardSearchFullComponent implements OnInit {
  yards: Yard[] = [];
  doors: Door[] = [];
  selectedDoor: Door | null = null;
  selectedYard: Yard | null = null;
  loading: boolean = false;
  yardTypes: string[] = [];
  selectedYardType: string = '';
  showDoorDropdown: boolean = false;

  @ViewChild('filter') filterInput!: ElementRef;
  @ViewChild('door') doorDropdown!: any;

  constructor(
    public ref: DynamicDialogRef,
    private yardService: YardService,
    private doorService: DoorService
  ) {}

  ngOnInit() {
    this.loadYards();
  }

  loadYards() {
    this.loading = true;
    this.yardService.getAll().subscribe({
      next: (data) => {
        this.yardTypes = [...new Set(data.map((yard) => yard.yardType))].filter(
          (type) => type !== 'Dock'
        );
        this.yards = data.filter(
          (yard) => yard.status === 'Available' && yard.yardType === 'Yard'
        );
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading yards:', error);
        this.loading = false;
      },
    });
  }

  loadFilteredYards() {
    this.loading = true;
    this.selectedYard = null;
    this.selectedDoor = null;
    this.showDoorDropdown = false;
    this.yardService.get_by_yardType(this.selectedYardType).subscribe({
      next: (data_from_type) => {
        this.yards = data_from_type.filter((yard) => yard.status === 'Available');
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading yard types:', error);
      },
    });
  }

  loadDoor() {
    this.doorService.getAll().subscribe({
      next: (data_from_door) => {
        this.doors = data_from_door;
        this.showDoorDropdown = true;
      },
    });
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  clear(table: Table) {
    table.clear();
    this.filterInput.nativeElement.value = '';
    this.selectedYardType = '';
    this.selectedYard = {} as Yard;
    this.selectedDoor = null;
    this.showDoorDropdown = false;
    this.loadYards();
  }

  refresh() {
    this.filterInput.nativeElement.value = '';
    this.selectedYardType = '';
    this.selectedYard = {} as Yard;
    this.selectedDoor = null;
    this.showDoorDropdown = false;
    this.loadYards();
  }

  onYardTypeChange() {
    this.loadFilteredYards();
  }

  onYardSelect(event: any) {
    console.log('onYardSelect', event);
    this.selectedYard = event.data;
    this.selectedDoor = null;
    this.showDoorDropdown = this.selectedYard?.yardType === 'Dock';
    if (this.showDoorDropdown) {
      this.loadDoor();
    }
  }

  canSelect(): boolean {
    if (this.selectedYard?.yardType === 'Dock') {
      return !!this.selectedDoor && !!this.selectedYard;
    }
    return !!this.selectedYard;
  }

  selectYard() {
    if (this.selectedYard) {
      const result = {
        yard: this.selectedYard,
        door: this.selectedDoor,
      };
      this.ref.close(result);
    }
  }

  cancel() {
    this.ref.close();
  }
}
