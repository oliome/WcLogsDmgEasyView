import { Component, OnInit, ViewChild, Input, Inject } from '@angular/core';
import { MatAccordion } from '@angular/material/expansion';
import { DOCUMENT } from '@angular/common';
import { MatFormFieldModule, MatInputModule } from '@angular/material';
import { Players } from "../model/player";
import { HttpClient, HttpHeaders } from '@angular/common/http';


@Component({
  selector: 'app-player-table',
  templateUrl: './player-table.component.html',
  styleUrls: ['./player-table.component.css']
})

export class PlayerTableComponent implements OnInit {
  @ViewChild(MatAccordion, { static: false }) accordion: MatAccordion;

  constructor(@Inject(DOCUMENT) private document: Document, private http: HttpClient) { }
  public header = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'Access-Control-Allow-Headers': 'Content-Type',
  });

  public endpoint = "/Logs/GenerateImage/";
  ngOnInit() {
  }

   async getDamage(id: number) {
    console.log(id);
    return (await this.http.get(window.location.origin + this.endpoint + this.report + "/" + id).toPromise());

   }

  beforePanelClosed(panel) {
    console.log(panel);
  }
   async beforePanelOpened(panel) {
     if (!panel.Checked || panel.Checked == undefined || panel.Image == undefined) {
       var result = await this.getDamage(panel.id);
       document.getElementById(panel.id.toString()).setAttribute("src", `data:image/png;base64,${result}`);
     }
    
    panel.Checked = true;
    
    
  }

  afterPanelClosed() {
    console.log("Panel closed!");
  }
  afterPanelOpened() {
    console.log("Panel opened!");
  }


  @Input() players: Players[];
  @Input() report: string;
  


}
