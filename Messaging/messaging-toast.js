class MessagingToast extends HTMLElement {
  #eventSource;
  #broadcastChannel = new BroadcastChannel('messages');
  #showDebug = localStorage.getItem('showDebugMessages') === 'true';
  #container;

  connectedCallback() {
    const baseUrl = this.getAttribute('api-base-url') ?? '';

    this.#container = document.createElement('div');
    this.#container.className = 'messaging-toast-container';
    this.appendChild(this.#container);

    this.#eventSource = new EventSource(`${baseUrl}/api/messages/stream`);
    this.#eventSource.onmessage = (e) => {
      const message = JSON.parse(e.data);
      if (message.severity === 'debug' && !this.#showDebug) return;
      this.#addMessage(message, baseUrl);
      if (message.severity === 'info') {
        setTimeout(() => this.#dismiss(message.id, baseUrl), 4000);
      }
    };

    this.#broadcastChannel.onmessage = (e) => {
      if (e.data.type === 'dismiss') {
        this.#removeToast(e.data.id);
      }
    };
  }

  disconnectedCallback() {
    this.#eventSource?.close();
    this.#broadcastChannel.close();
  }

  #addMessage(message, baseUrl) {
    const toast = document.createElement('div');
    toast.className = `messaging-toast-item ${message.severity}`;
    toast.dataset.id = message.id;

    const segments = document.createElement('span');
    segments.className = 'messaging-toast-segments';
    for (const segment of message.segments) {
      if (segment.url) {
        const a = document.createElement('a');
        a.href = segment.url;
        a.textContent = segment.text;
        segments.appendChild(a);
      } else {
        segments.appendChild(document.createTextNode(segment.text));
      }
    }
    toast.appendChild(segments);

    const button = document.createElement('button');
    button.className = 'messaging-toast-dismiss';
    button.textContent = '✕';
    button.onclick = () => this.#dismiss(message.id, baseUrl);
    toast.appendChild(button);

    this.#container.appendChild(toast);
  }

  #dismiss(id, baseUrl) {
    this.#removeToast(id);
    this.#broadcastChannel.postMessage({ type: 'dismiss', id });
    fetch(`${baseUrl}/api/messages/${id}/acknowledge`, { method: 'POST' });
  }

  #removeToast(id) {
    this.#container.querySelector(`[data-id="${id}"]`)?.remove();
  }
}

customElements.define('messaging-toast', MessagingToast);