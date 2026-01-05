import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Shunt } from '@core/models/master/shunt.model';
import { ShuntService } from '@core/services/master/shunt.service';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-shunt-search-full',
  templateUrl: './shunt-search-full.component.html',
})
export class ShuntSearchFullComponent implements OnInit {
  shunts: Shunt[] = [];
  selectedShunt: Shunt | null = null;
  loading: boolean = false;
  statuses: string[] = ['Available', 'In-Use'];
  selectedStatus: string = '';
  @ViewChild('filter') filterInput!: ElementRef;

  constructor(
    public ref: DynamicDialogRef,
    private shuntService: ShuntService
  ) {}

  ngOnInit() {
    this.loadShunts();
  }

  loadShunts() {
    this.loading = true;
    this.shuntService.getAll().subscribe({
      next: (data) => {
        this.shunts = data.filter((trailer) => trailer.status === 'Available');
        this.selectedStatus = 'Available';
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading shunts:', error);
        this.loading = false;
      },
    });
  }

  onStatusChange() {
    if (this.selectedStatus) {
      this.loadFilteredShunts();
    } else {
      this.loadShunts();
    }
  }

  loadFilteredShunts() {
    this.loading = true;
    this.shuntService.get_by_status(this.selectedStatus).subscribe({
      next: (data) => {
        this.shunts = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading filtered shunts:', error);
        this.loading = false;
      },
    });
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, 'contains');
  }

  clear(table: Table) {
    table.clear();
    this.filterInput.nativeElement.value = '';
    this.selectedStatus = '';
    this.selectedShunt = {} as Shunt;
    this.loadShunts();
  }

  refresh() {
    this.filterInput.nativeElement.value = '';
    this.selectedStatus = '';
    this.selectedShunt = {} as Shunt;
    this.loadShunts();
  }

  getSeverity(status: string): string {
    switch (status) {
      case 'Available':
        return 'success';
      case 'In-Use':
        return 'danger';
      default:
        return 'info';
    }
  }

  selectShunt() {
    if (this.selectedShunt) {
      this.ref.close(this.selectedShunt);
    }
  }
  cancel() {
    this.ref.close();
  }
}
