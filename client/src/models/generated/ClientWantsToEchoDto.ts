
class ClientWantsToEchoDto {
  private _eventType?: string;
  private _message?: string;

  constructor(input: {
    eventType?: string,
    message?: string,
  }) {
    this._eventType = input.eventType;
    this._message = input.message;
  }

  get eventType(): string | undefined { return this._eventType; }
  set eventType(eventType: string | undefined) { this._eventType = eventType; }

  get message(): string | undefined { return this._message; }
  set message(message: string | undefined) { this._message = message; }
}
export default ClientWantsToEchoDto;
