import { Component, OnInit } from '@angular/core';
import { RoomService } from '../../../services/room.service';
import { IRoom } from '../../../models/IRoom';
import * as lodash from "lodash";

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.sass']
})
export class RoomComponent implements OnInit {
  rooms: Array<IRoom> = [];
  roomID: string [];

  constructor(private roomService: RoomService) { }

  ngOnInit() {
    this.getAllRooms();
  }

  private getAllRooms(): void {
    this.roomService.getRooms().subscribe(
      rooms => {
        this.rooms = rooms;
      }
    );
  }

  addRoom(roomID: string) {
    const room = <IRoom> {
      roomID: roomID,
      slots: []
    }
    this.roomService.addRoom(room).subscribe(newRoom => {
      this.rooms.push(newRoom);
    });
  }

  // TODO no idea what this does
  editRoom(room: IRoom) {
    this.roomService.editRoom(room).subscribe(editedRoom => {
      this.rooms = lodash.map(this.rooms, currentRoom => {
        (currentRoom.roomID === editedRoom.roomID) ? editedRoom : currentRoom;
      })
    });
  }

}
