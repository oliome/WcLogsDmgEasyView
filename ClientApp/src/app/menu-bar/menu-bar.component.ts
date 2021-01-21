import { Component, OnInit } from '@angular/core';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Players } from '../model/player';
@Component({
  selector: 'app-menu-bar',
  templateUrl: './menu-bar.component.html',
  styleUrls: ['./menu-bar.component.css']
})
export class MenuBarComponent implements OnInit {
  public header = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'Access-Control-Allow-Headers': 'Content-Type',
  });
  public endpoint = "/Logs/GetFromLogAsync/";
  public players: Players[];
  public report: string;
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  getReport(reportId) {

    this.http.get<Players[]>(window.location.origin + this.endpoint + reportId).subscribe(val => {
      this.players = val;
    });
    this.players = this.players;
    this.report = reportId;

  }
}
