
class ServerSendsEchoDto {
  private _eventType?: string;
  private _message?: string;
  private _client?: string;

  constructor(input: {
    eventType?: string,
    message?: string,
    client?: string,
  }) {
    this._eventType = input.eventType;
    this._message = input.message;
    this._client = input.client;
  }

  get eventType(): string | undefined { return this._eventType; }
  set eventType(eventType: string | undefined) { this._eventType = eventType; }

  get message(): string | undefined { return this._message; }
  set message(message: string | undefined) { this._message = message; }

  get client(): string | undefined { return this._client; }
  set client(client: string | undefined) { this._client = client; }
}
export default ServerSendsEchoDto;
