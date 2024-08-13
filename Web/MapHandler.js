export default class MapHandler {

    static Instance = null;
    roomDivMap = new Map();
    transPathMap = new Map();
    mouseX = 0;
    mouseY = 0;
    offset = [0, 0];
    rootDiv = null;
    isDown = false;
    scale = 1;

    drag = (evt) => {

        if (this.getRoomDataFromPoint(evt.clientX, evt.clientY) !== null) {
            return;
        }
        const el = this.rootDiv;
        el.style.touchAction = "none";
        const move = (evt) => {
            el.style.left = `${el.offsetLeft + evt.movementX}px`;
            el.style.top = `${el.offsetTop + evt.movementY}px`;
        };

        const up = () => {
            removeEventListener("pointermove", move);
            removeEventListener("pointerup", up);
        };

        addEventListener("pointermove", move);
        addEventListener("pointerup", up);
        //evt.preventDefault();

    };

    zoom = (e) => {


        // Restrict scale

        const rect = this.rootDiv.getBoundingClientRect();


        let relX = (rect.left - this.mouseX) / this.scale;
        let relY = (rect.top - this.mouseY) / this.scale;


        this.scale += e.deltaY * -0.001;
        this.scale = Math.min(Math.max(0.1, this.scale), 3);
        this.rootDiv.style.transform = `scale(${this.scale})`;

        this.rootDiv.style.left = `${this.mouseX + relX * this.scale}px`;
        this.rootDiv.style.top = `${this.mouseY + relY * this.scale}px`;

        this.updateAllConnections();

    };



    getRelativeBoundingClientRect(element, parent) {
        const elementRect = element.getBoundingClientRect();
        const parentRect = parent.getBoundingClientRect();

        return {
            top: elementRect.top - parentRect.top,
            right: elementRect.right - parentRect.left,
            bottom: elementRect.bottom - parentRect.top,
            left: elementRect.left - parentRect.left,
            width: elementRect.width,
            height: elementRect.height
        };
    }

    constructor() {

        MapHandler.Instance = this;
        this.x = 0;
        this.y = 0;

        this.rootDiv = document.createElement("div");
        this.rootDiv.style.position = "absolute";
        this.rootDiv.style.left = "0px";
        this.rootDiv.style.top = "0px";
        this.rootDiv.style.width = "100%";
        this.rootDiv.style.height = "100%";
        this.rootDiv.style.background = "ffffff00";
        this.rootDiv.style.color = "blue";
        this.rootDiv.style.transformOrigin = "top left";
        this.rootDiv.style.pointerEvents = "auto";
        var eventDiv = document.createElement("div");
        eventDiv.style.position = "absolute";
        eventDiv.style.left = "0px";
        eventDiv.style.top = "0px";
        eventDiv.style.width = "100%";
        eventDiv.style.height = "100%";
        eventDiv.style.zIndex = 500;
        eventDiv.addEventListener("pointerdown", this.drag);
        document.body.appendChild(this.rootDiv);
        document.body.appendChild(eventDiv);

        document.body.addEventListener('wheel', this.zoom);
        document.body.addEventListener("mousemove", this.updateMouse, false);
        document.body.addEventListener("mouseenter", this.updateMouse, false);
        document.body.addEventListener("mouseleave", this.updateMouse, false);

        this.setupRoomDrag(eventDiv);
    }

    clickRoom(room) {
        console.log('clicked room', room.innerText);
    }

    updateMouse(event) {
        MapHandler.Instance.mouseX = event.pageX;
        MapHandler.Instance.mouseY = event.pageY;
    }
    loadNewTransition(data) {
        const transition = JSON.parse(data);
        this.createRoom(transition.fromRoom);
        this.createRoom(transition.toRoom);
        this.addConnection(transition);
    }

    loadMap(data) {
        const parsedData = JSON.parse(data);

        for (let i = 0; i < parsedData.length; i++) {
            let transition = parsedData[i];
            this.loadNewTransition(transition);
        }

        
    }

    roomExists(room) {
        for (const key of this.roomDivMap.keys()) {
            if (key === room) {
                return true;
            }
        }
        return false;
    }

    createRoom(room) {
        if (!this.roomDivMap.has(room.roomName)) {
            const newDiv = document.createElement('div');
            newDiv.style.position = 'absolute';
            newDiv.id = trans.toRoom;
            newDiv.className = 'room';
            newDiv.innerText = room.roomName;
            newDiv.style.backgroundColor = 'lightblue';
            newDiv.style.padding = '10px';
            newDiv.style.left = room.mapPosX + '%';
            newDiv.style.top = room.mapPosY + '%';
            newDiv.style.width = room.width / 10 + '%';
            newDiv.style.height = room.height / 10 + '%';
            newDiv.style.pointerEvents = 'auto';
            this.rootDiv.appendChild(newDiv);
            this.roomDivMap.set(trans.toRoom, newDiv);
        }
    }
    /*
    createRooms(trans, x, y) {
        if (!this.roomDivMap.has(trans.toRoom)) {


            const newDiv = document.createElement('div');
            newDiv.style.position = 'absolute';
            newDiv.id = trans.toRoom;
            newDiv.className = 'room';
            newDiv.innerText = trans.toRoom;
            newDiv.style.backgroundColor = 'lightblue';
            newDiv.style.padding = '10px';
            newDiv.style.left = x + '%';
            newDiv.style.top = y + '%';
            newDiv.style.width = trans.toRoomWidth / 10 + '%';
            newDiv.style.height = trans.toRoomHeight / 10 + '%';
            newDiv.style.pointerEvents = 'auto';
            this.rootDiv.appendChild(newDiv);
            //this.makeDraggable(newDiv);


            this.roomDivMap.set(trans.toRoom, newDiv);
        }

        if (!this.roomDivMap.has(trans.fromRoom)) {


            const newDiv = document.createElement('div');
            newDiv.style.position = 'absolute';
            newDiv.id = trans.fromRoom;
            newDiv.className = 'room';
            newDiv.innerText = trans.fromRoom;
            newDiv.style.backgroundColor = 'lightblue';
            newDiv.style.padding = '10px';
            newDiv.style.left = x + '%';
            newDiv.style.top = y + '%';
            newDiv.style.width = trans.toRoomWidth / 10 + '%';
            newDiv.style.height = trans.toRoomHeight / 10 + '%';
            newDiv.style.pointerEvents = 'auto';
            this.rootDiv.appendChild(newDiv);
            //this.makeDraggable(newDiv);


            this.roomDivMap.set(trans.fromRoom, newDiv);
        }
    }
    */
    setupRoomDrag(eventDiv) {

        let offsetX, offsetY, isDragging = false;
        let dragElement = null;
        document.addEventListener('mousedown', (event) => {

            let data = this.getRoomDataFromPoint(event.clientX, event.clientY);
            if (data !== null) {
                dragElement = data.room;
                const rect = this.rootDiv.getBoundingClientRect();
                const x = event.clientX - rect.left;
                const y = event.clientY - rect.top;
                console.log('clicked body');
                // Adjust the click position based on the scaling factor
                const scaledX = (x / this.scale);
                const scaledY = (y / this.scale);
                offsetX = rect.left + event.clientX - rect.left - data.left * this.scale;
                offsetY = rect.top + event.clientY - rect.top - data.top * this.scale;
                isDragging = true;


            }
            event.preventDefault();
        });

        document.addEventListener('mousemove', (event) => {
            if (isDragging) {
                const x = (event.clientX - offsetX) / this.scale;
                const y = (event.clientY - offsetY) / this.scale;
                dragElement.style.left = `${x}px`;
                dragElement.style.top = `${y}px`;
            }
            MapHandler.Instance.updateAllConnections();
            event.preventDefault();

        });


        document.addEventListener('mouseup', () => {
            isDragging = false;

        });
    }

    getRoomDataFromPoint(pointx, pointy) {

        const rect = this.rootDiv.getBoundingClientRect();
        const x = pointx - rect.left;
        const y = pointy - rect.top;
        console.log('clicked body');
        // Adjust the click position based on the scaling factor
        const scaledX = (x / this.scale);
        const scaledY = (y / this.scale);

        // Find the target element based on the scaled coordinates
        //const targetElement = document.elementFromPoint(scaledX, scaledY);


        for (let val of this.roomDivMap.values()) {
            const left = this.getRelativeBoundingClientRect(val, this.rootDiv).left / this.scale;
            const top = this.getRelativeBoundingClientRect(val, this.rootDiv).top / this.scale;
            const width = this.getRelativeBoundingClientRect(val, this.rootDiv).width / this.scale;
            const height = this.getRelativeBoundingClientRect(val, this.rootDiv).height / this.scale;
            if (scaledX > left && scaledX < left + width && scaledY > top && scaledY < top + height) {
                return {
                    room: val,
                    left: left,
                    top: top,
                    width: width,
                    height: height
                };

            }
        }
        return null;
    }

    addConnection(trans) {
        const SVG_NS = 'http://www.w3.org/2000/svg';
        let fromDiv = this.roomDivMap.get(trans.fromRoom.roomName);
        let toDiv = this.roomDivMap.get(trans.toRoom.roomName);
        let svg = document.createElementNS(SVG_NS, 'svg');
        svg.setAttribute("class", "svg")
        let path = document.createElementNS(SVG_NS, 'path');
        path.setAttribute("class", "path")
        this.transPathMap.set(trans, path);
        this.updateConnection(trans);
        svg.appendChild(path);
        this.rootDiv.appendChild(svg);

      
    }

    updateConnection(trans) {

        let path = this.transPathMap.get(trans);
        let fromDiv = this.roomDivMap.get(trans.fromRoom.roomName);
        let toDiv = this.roomDivMap.get(trans.toRoom.roomName);

        let x1 = this.getRelativeBoundingClientRect(fromDiv, this.rootDiv).left / this.scale;
        let y1 = this.getRelativeBoundingClientRect(fromDiv, this.rootDiv).top / this.scale;
        let x4 = this.getRelativeBoundingClientRect(toDiv, this.rootDiv).left / this.scale;
        let y4 = this.getRelativeBoundingClientRect(toDiv, this.rootDiv).top / this.scale;
        let x2 = x1;
        let x3 = x4;
        let data = `M${x1} ${y1} C ${x2} ${y1} ${x3} ${y4} ${x4} ${y4}`;
        //path.setAttributeNS(null, 'stroke-width', 6 * this.scale)
        path.setAttributeNS(null, 'd', data);
    }


    updateAllConnections() {
        for (let trans of this.transPathMap.keys()) {
            this.updateConnection(trans);
        }
    }

}
