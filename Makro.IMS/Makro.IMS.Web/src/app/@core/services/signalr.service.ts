import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  hubConnection: any;
  supplierHubConnection: any;

  constructor() { }

  startConnection() {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}/userregister`)          
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('เชื่อมต่อท่อสำเร็จ'))
      .catch((err: any) => console.log('มีปัญหาในการเชื่อมต่อท่อ: ' + err));


    this.supplierHubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}/supplier`)      
      .build();

    this.supplierHubConnection
      .start()
      .then(() => console.log('เชื่อมต่อท่อสำเร็จ supplier'))
      .catch((err: any) => console.log('มีปัญหาในการเชื่อมต่อท่อ: ' + err));


    }



}