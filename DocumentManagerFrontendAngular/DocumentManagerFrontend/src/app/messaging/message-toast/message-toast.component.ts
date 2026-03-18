import {Component, effect, inject} from '@angular/core';
import {MessageService} from "../message.service";
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-message-toast2',
  imports: [
    RouterLink
  ],
  templateUrl: './message-toast.component.html',
  styleUrl: './message-toast.component.scss',
})
export class MessageToastComponent {
  messageService = inject(MessageService);
  constructor() {
    effect(() => {
      const messages = this.messageService.messages();
      messages
        .filter(m => m.severity === 'info')
        .forEach(m => setTimeout(() => this.messageService.dismiss(m.id), 4000));
    });
  }
}
