import { NgModule }       from '@angular/core';
import { CommonModule }   from '@angular/common';
import { FormsModule }    from '@angular/forms';

import { DashboardComponent } from './dashboard.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { RoomComponent } from './room/room.component';
import { BookingComponent } from './booking/booking.component';

@NgModule({
  imports: [
    DashboardRoutingModule
  ],
  declarations: [
  ]
})
export class DashboardModule {}


/*
Copyright 2017-2018 Google Inc. All Rights Reserved.
Use of this source code is governed by an MIT-style license that
can be found in the LICENSE file at http://angular.io/license
*/