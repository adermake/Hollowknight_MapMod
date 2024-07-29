
import MapHandler from './MapHandler.js';

export default class App {
    ws = null
    map = null;
    constructor(map) {
        this.map = map;
    }

    async start() {

        this.wsConnect()

    }

    wsConnect() {
        if (this.ws) return

        this.ws = new WebSocket("ws://" + location.host + "/ws")
        this.ws.addEventListener("open", ev => {
            console.log("Connected to server")
        })
        this.ws.addEventListener("message", ev => {
            this.map.loadMap(ev.data)
            console.log(ev.data)
            //this.handleMessage(JSON.parse(ev.data))
        })
        this.ws.addEventListener("close", ev => {
            this.ws = null
            this.unloadSave()
            this._setBlockingMessage("No game running")
        })
        this.ws.addEventListener("error", ev => {
            this.ws = null
            this.unloadSave()
            this._setBlockingMessage("No game running")
        })
    }


    // Use like:

}