import {useEffect} from "react";
import useWebSocket, {ReadyState} from "react-use-websocket";
import {useAtom} from "jotai";
import {EchoAtom} from "./Atoms.ts";



export default function useCustomWsClient() {
    const [history, setHistory] = useAtom(EchoAtom);
     const {lastMessage, sendJsonMessage, readyState} = useWebSocket('ws://localhost:8181');

     const connectionStatus = {
        [ReadyState.CONNECTING]: 'Connecting',
        [ReadyState.OPEN]: 'Open',
        [ReadyState.CLOSING]: 'Closing',
        [ReadyState.CLOSED]: 'Closed',
        [ReadyState.UNINSTANTIATED]: 'Uninstantiated',
    }[readyState];
    useEffect(() => {
        if (lastMessage !== null) {
            const message = lastMessage.data ? JSON.parse(lastMessage.data) : {};
            setHistory([...history, message]);
        }
    }, [lastMessage]);

    return {sendJsonMessage, connectionStatus};
}
