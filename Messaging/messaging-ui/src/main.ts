import { createApplication } from '@angular/platform-browser';
import { createCustomElement } from '@angular/elements';
import { MessageToastComponent } from './app/message-toast/message-toast.component';
import { provideHttpClient } from '@angular/common/http';

const app = await createApplication({
  providers: [provideHttpClient()],
});
const toastElement = createCustomElement(MessageToastComponent, { injector: app.injector });
customElements.define('messaging-toast', toastElement);
