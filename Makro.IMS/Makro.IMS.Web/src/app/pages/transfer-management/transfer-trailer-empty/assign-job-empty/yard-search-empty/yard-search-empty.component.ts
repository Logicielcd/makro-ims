import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Trailer } from '@core/models/master/trailer.model';
import { Yard } from '@core/models/master/yard.model';
import { YardService } from '@core/services/master/yard.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-yard-search-empty',
  templateUrl: './yard-search-empty.component.html',
})
export class YardSearchEmptyComponent implements OnInit {
  trailer: Trailer;
  yards: Yard[] = [];
  selectedYard: Yard;
  loading: boolean = false;
  yardTypes: string[] = [];
  selectedYardType: string = '';

  @ViewChild('filter') filterInput!: ElementRef;
  @ViewChild('door') doorDropdown!: any;

  constructor(
    public ref: DynamicDialogRef,
    private yardService: YardService,
    public config: DynamicDialogConfig
  ) {}

  ngOnInit() {
    this.trailer = this.config.data?.trailer;
    console.log(this.trailer);
    this.loadYards();
  }

  loadYards() {
    this.loading = true;
    this.yardService.getAll().subscribe({
      next: (data) => {
        this.yardTypes = [...new Set(data.map((yard) => yard.yardType))];
        this.yards = data.filter((yard) => {
          if (!yard.trailerType || !this.trailer?.trailerType) {
            return false;
          }
          return (
            yard.status === 'Available' &&
            yard.yardType === 'Dock' &&
            yard.trailerType
              .split('|')
              .map((type) => type.trim())
              .includes(this.trailer.trailerType)
          );
        });

        this.loading = false;
      },
    });
  }

  loadFilteredYards() {
    this.loading = true;
    this.selectedYard = null;
    this.yardService.get_by_yardType(this.selectedYardType).subscribe({
      next: (data_from_type) => {
        // ถ้าเลือก Type เป็น Dock
        if (this.selectedYardType === 'Dock') {
          this.yards = data_from_type.filter((yard) => {
            if (!yard.trailerType || !this.trailer?.trailerType) {
              return false;
            }
            return (
              yard.status === 'Available' &&
              yard.yardType === 'Dock' &&
              yard.trailerType
                .split('|')
                .map((type) => type.trim())
                .includes(this.trailer.trailerType)
            );
          });
        } else {
          this.yards = data_from_type.filter(
            (yard) => yard.status === 'Available'
          );
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading yard types:', error);
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
    this.selectedYard = null;
    this.loadYards();
  }

  refresh() {
    this.filterInput.nativeElement.value = '';
    this.selectedYardType = '';
    this.selectedYard = null;
    this.loadYards();
  }

  onYardTypeChange() {
    this.loadFilteredYards();
  }

  onYardSelect(event: any) {
    this.selectedYard = event.data;
  }

  selectYard() {
    if (this.selectedYard) {
      const result = {
        yard: this.selectedYard,
      };
      this.ref.close(result);
    }
  }

  cancel() {
    this.ref.close();
  }
}
