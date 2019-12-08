import { Component, OnInit, Input } from '@angular/core';
import { requestDetails } from 'src/app/models/requestDetails';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent {
  @Input() public requestDetailsModel: requestDetails;
}
