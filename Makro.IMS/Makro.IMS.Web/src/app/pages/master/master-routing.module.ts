import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'warehouse',
    loadChildren: () =>
      import('./warehouse/warehouse.module').then((m) => m.WarehouseModule),
  },
  {
    path: 'operation-type',
    loadChildren: () =>
      import('./operation-type/operation-type.module').then(
        (m) => m.OperationTypeModule
      ),
  },
  {
    path: 'door',
    loadChildren: () => import('./door/door.module').then((m) => m.DoorModule),
  },
  {
    path: 'warehouse-capacity',
    loadChildren: () =>
      import('./warehouse-capacity/warehouse-capacity.module').then(
        (m) => m.WarehouseCapacityModule
      ),
  },
  {
    path: 'warehouse-operation-capacity',
    loadChildren: () =>
      import(
        './warehouse-operation-capacity/warehouse-operation-capacity.module'
      ).then((m) => m.WarehouseOperationCapacityModule),
  },
  {
    path: 'supplier-group',
    loadChildren: () =>
      import('./supplier-group/supplier-group.module').then(
        (m) => m.SupplierGroupModule
      ),
  },
  {
    path: 'supplier',
    loadChildren: () =>
      import('./supplier/supplier.module').then((m) => m.SupplierModule),
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MasterRoutingModule {}
