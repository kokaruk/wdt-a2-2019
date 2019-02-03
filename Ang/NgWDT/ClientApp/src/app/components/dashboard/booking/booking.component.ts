import { Component, OnInit } from '@angular/core';
import { BookingService } from '../../../services/booking.service';
import { ISlot } from '../../../models/ISlot';
import * as lodash from 'lodash';
import { IUser } from '../../../models/IUser';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.sass']
})
export class BookingComponent implements OnInit {
  slots: Array<ISlot> = [];
  displaySlots: Array<ISlot> = [];
  users: Array<IUser> = [];
  students: Array<IUser> = [];
  staff: Array<IUser> = [];
  staffToggle: boolean = true;
  selectedUser: IUser;

  constructor(
    private bookingService: BookingService,
    private userService: UserService
  ) { }

  ngOnInit() {
    // get the slots
    this.getAllUsers();
    this.getAllSlots();
    this.modifyDisplay();
  }

  deleteSlot(slot: ISlot): void {
    console.log(`deleting slot`, slot);
    this.slots = lodash.filter(this.slots, currentSlot => {
      return !this.slotPrimaryKeyPredicate(currentSlot, slot);
    } );
    // this.bookingService.deleteSlot(slot); TODO uncomment this
  }

  editSlot(slot: ISlot): void {
    console.log(`editing slot`, slot);
    this.bookingService.editSlot(slot).subscribe(editedSlot => {
      this.slots = lodash.map(this.slots, currentSlot => {
        this.slotPrimaryKeyPredicate(currentSlot, editedSlot) ? editedSlot : currentSlot;
      });
    });
  }

  toggleStaff(): void {
    this.staffToggle = !this.staffToggle;
    this.selectedUser = undefined;
  }

  selectUser($event) {
    this.selectedUser = $event.value;
    this.modifyDisplay();
  }

  private slotPrimaryKeyPredicate(slot1: ISlot, slot2: ISlot): boolean {
    return slot1.roomID === slot2.roomID && slot1.startTime === slot2.startTime;
  }

  private getAllSlots(): void {
    this.bookingService.getAllSlots().subscribe(
      slots => {
        this.slots = slots;
        this.displaySlots = slots;
      }
    );
  }

  private getAllUsers(): void {
    this.userService.getAllUsers().subscribe(users => {
      this.users = users;
      this.students = lodash.filter(this.users, user => user.userID.charAt(0) === 's'); // TODO move this to user service
      this.staff = lodash.filter(this.users, user => user.userID.charAt(0) === 'e'); // TODO move this to user service
    });
  }

  private modifyDisplay(): void {
    (this.selectedUser != undefined) ? 
    this.displaySlots = lodash.filter(this.slots, slot => slot.staffID === this.selectedUser || slot.studentID === this.selectedUser)
    : this.displaySlots = this.slots;
  }
}
