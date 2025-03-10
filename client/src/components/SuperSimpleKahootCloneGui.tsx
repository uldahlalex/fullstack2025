import {useWsClient} from "ws-request-hook";
import {useEffect, useState} from "react";
import {randomUid} from "./App.tsx";


export default function SuperSimpleKahootCloneGui() {
    
    const {readyState, onMessage} = useWsClient();
    const [lobby, setLobby] = useState<string[]>([])
    const [gameId, setGameIdId] = useState<string | undefined>(undefined)
    const [gameState, setGameState] = useState<any>()
    const [answer, setAnswer] = useState<string>('easv');
    useEffect(() => {
        if (readyState!=1) return;
        onMessage<any>("lobby", (dto) => {
            setLobby(dto.allClientIds)
        })
        onMessage<any>("game", (dto) => {
            setGameIdId(dto.gameId)
        })
        onMessage<any>("round", (dto) => {
            setGameState(dto)
        })
    }, [readyState]);
    
    return (
        <>
            <hr />
            <div className="flex flex-col">

            <div>lobby users:</div>
            {JSON.stringify(lobby)}

            {
                gameId && <div>Game ID: {JSON.stringify(gameId)}</div>
            }
            {
                gameState && <div>Game state: {JSON.stringify(gameState)}</div>
            }
            <hr />
                <button className="btn" onClick={e => fetch('http://localhost:8080/JoinLobby?clientId='+randomUid, {method: 'POST'})}>Enter lobby</button>
            <button className="btn" onClick={e => fetch('http://localhost:8080/StartGame', {method: 'POST'})}>Start game for lobby users</button>
            <button className="btn" onClick={e => fetch('http://localhost:8080/PlayThroughRounds', {method: 'POST'})}>Start next round (if any)</button>

                <span>Answer: <input className="input" value={answer} onChange={e => setAnswer(e.target.value)} /></span>
            <button className="btn" onClick={e => fetch('http://localhost:8080/SubmitAnswer?player='+randomUid+"&answer="+answer+"&questionId="+gameState.questionId,
                {method: 'POST'})}>Submit answer</button>
        </div></>)
} 