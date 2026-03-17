import {Injectable, signal} from '@angular/core';

type MessageSeverity = 'debug' | 'info' | 'warning' | 'error';
export interface MessageSegment {
  text: string;
  url?: string;
}

export interface Message {
  id: string;
  segments: MessageSegment[];
  severity: MessageSeverity;
  createdAt: string;
}

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  messages = signal<Message[]>([]);
  private broadcastChannel = new BroadcastChannel('messages');
  private eventSource?: EventSource;
  private showDebug = localStorage.getItem('showDebugMessages') === 'true';

  constructor() {
    if (localStorage.getItem('showDebugMessages') == null) {
      localStorage.setItem('showDebugMessages', 'false');
    }
    this.connect();
    this.broadcastChannel.onmessage = (e) => {
      if (e.data.type === 'dismiss') {
        this.removeMessage(e.data.id);
      }
    };
  }

  private connect() {
    this.eventSource = new EventSource('api/messages/stream');
    this.eventSource.onmessage = (e) => {
      const message: Message = JSON.parse(e.data);
      if (message.severity !== 'debug' || this.showDebug) {
        this.messages.update(msgs => [...msgs, { ...message }]);
      }
    };
  }

  dismiss(id: string) {
    this.removeMessage(id);
    this.broadcastChannel.postMessage({ type: 'dismiss', id });
  }

  private removeMessage(id: string) {
    this.messages.update(msgs => msgs.filter(m => m.id !== id));
  }
}
