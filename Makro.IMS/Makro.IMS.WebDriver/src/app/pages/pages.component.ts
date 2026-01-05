import { Component } from '@angular/core';
import { UntilDestroy } from '@ngneat/until-destroy';

@UntilDestroy()
@Component({
  selector: 'app-pages',
  template: `<app-layout></app-layout>`,
})
export class PagesComponent {}
