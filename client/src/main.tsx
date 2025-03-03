import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import App from "./components/App.tsx";
import {WsClientProvider} from "ws-request-hook";
import {Toaster} from "react-hot-toast";


createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <WsClientProvider url="ws://localhost:8080">
          <App />
      </WsClientProvider>
      <Toaster />
  </StrictMode>,
)
