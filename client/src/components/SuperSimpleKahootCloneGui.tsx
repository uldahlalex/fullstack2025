import {useWsClient} from "ws-request-hook";
import {useEffect, useState} from "react";
import {randomUid} from "./App.tsx";


export default function SuperSimpleKahootCloneGui() {
    
    const {readyState, onMessage} = useWsClient();
    const [lobby, setLobby] = useState<string[]>([])
    const [game, setgame] = useState<string | undefined>(undefined)
    const [gameState, setGameState] = useState<any>({})
    const [answer, setAnswer] = useState<string>('');
    useEffect(() => {
        console.log(randomUid);
        if (readyState!=1) return;
        onMessage<any>("lobby", (dto) => {
            setLobby(dto.allClientIds)
        })
        onMessage<any>("game", (dto) => {
            setgame(dto.gameId)
        })
        onMessage<any>("round", (dto) => {
            setGameState(dto.result)
        })
    }, [readyState]);
    
    return (<div className="flex">

        <div>lobby users:</div>
        {JSON.stringify(lobby)}

        {
            game && <div>{JSON.stringify(game)}</div> && <div>{JSON.stringify(gameState)}</div>
        }
        <hr />
        <button className="btn" onClick={e => fetch('http://localhost:8080/JoinLobby?clientId='+randomUid, {method: 'POST'})}>Enter lobby</button>
        <button className="btn" onClick={e => fetch('http://localhost:8080/StartGame', {method: 'POST'})}>Start game lobby</button>
        <button className="btn" onClick={e => fetch('http://localhost:8080/PlayThroughRounds', {method: 'POST'})}>Start next round (if any)</button>
        
        <input className="input" value={answer} onChange={e => setAnswer(e.target.value)} />
        <button className="btn" onClick={e => fetch('http://localhost:8080/SubmitAnswer?player='+randomUid+"&answer="+answer+"&questionId="+gameState.questionId, 
            {method: 'POST'})}>Submit answer</button>
    </div>)
} 