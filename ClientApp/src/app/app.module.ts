import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { MenuBarComponent } from './menu-bar/menu-bar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { PlayerTableComponent } from './player-table/player-table.component';
import { MatExpansionModule } from '@angular/material/expansion';


@NgModule({
  declarations: [
    AppComponent,
    MenuBarComponent,
    PlayerTableComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    BrowserAnimationsModule,
    MatExpansionModule
  ],
  providers: [
    PlayerTableComponent],
  exports: [MenuBarComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
