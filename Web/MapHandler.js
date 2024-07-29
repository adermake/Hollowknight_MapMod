export default class MapHandler {

    static Instance = null;
    roomDivMap = new Map();

    mouseX = 0;
    mouseY = 0;
    offset = [0, 0];
    rootDiv = null;
    isDown = false;
    scale = 1;

    drag = (evt) => {

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
        evt.preventDefault();

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
        this.rootDiv.style.background = "red";
        this.rootDiv.style.color = "blue";
        this.rootDiv.style.transformOrigin = "top left";
        this.rootDiv.style.pointerEvents = "auto";
        var eventDiv = document.createElement("div");
        eventDiv.style.position = "absolute";
        eventDiv.style.left = "0px";
        eventDiv.style.top = "0px";
        eventDiv.style.width = "100%";
        eventDiv.style.height = "100%";
        eventDiv.style.zIndex = 5;
        eventDiv.addEventListener("pointerdown", this.drag);
        document.body.appendChild(this.rootDiv);
        document.body.appendChild(eventDiv);

        document.body.addEventListener('wheel', this.zoom);
        document.body.addEventListener("mousemove", this.updateMouse, false);
        document.body.addEventListener("mouseenter", this.updateMouse, false);
        document.body.addEventListener("mouseleave", this.updateMouse, false);

        document.body.addEventListener('click', (event) => {
            // Calculate the click position relative to the parent div
            const rect = this.rootDiv.getBoundingClientRect();
            const x = event.clientX - rect.left;
            const y = event.clientY - rect.top;
            console.log('clicked body');
            // Adjust the click position based on the scaling factor
            const scaledX = (x / this.scale);
            const scaledY = (y / this.scale);

            // Find the target element based on the scaled coordinates
            //const targetElement = document.elementFromPoint(scaledX, scaledY);
            const test = document.createElement('div');
            test.style.width = "10px";
            test.style.height = "10px";
            test.style.position = 'absolute';
            test.style.left = scaledX + 'px';
            test.style.top = scaledY + 'px';
            test.style.backgroundColor = 'yellow';
            this.rootDiv.appendChild(test);

            for (let val of this.roomDivMap.values()) {
                const left = this.getRelativeBoundingClientRect(val, this.rootDiv).left / this.scale;
                const top = this.getRelativeBoundingClientRect(val, this.rootDiv).top / this.scale;
                const width = this.getRelativeBoundingClientRect(val, this.rootDiv).width / this.scale;
                const height = this.getRelativeBoundingClientRect(val, this.rootDiv).height / this.scale;
                if (scaledX > left && scaledX < left + width && scaledY > top && scaledY < top + height) {
                    this.clickRoom(val);
                }
            }


        });

    }

    clickRoom(room) {
        console.log('clicked room', room.innerText);
    }

    updateMouse(event) {
        MapHandler.Instance.mouseX = event.pageX;
        MapHandler.Instance.mouseY = event.pageY;
    }

    loadMap(data) {
        const parsedData = JSON.parse(data);

        for (let i = 0; i < parsedData.length; i++) {
            let transition = parsedData[i];

            if (!this.roomExists(transition.toRoom)) {
                this.createRoom(transition, this.x, this.y);
                this.x += 20;
                this.y += 20;
            }
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

    createRoom(trans, x, y) {
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



    makeDraggable(element) {
        let offsetX, offsetY, isDragging = false;

        element.addEventListener('click', (e) => {
            console.log('Child clicked!');
        });
        element.addEventListener('mousedown', (e) => {
            isDragging = true;
            offsetX = e.clientX - element.getBoundingClientRect().left;
            offsetY = e.clientY - element.getBoundingClientRect().top;
            element.style.cursor = 'grabbing';
        });

        document.addEventListener('mousemove', (e) => {
            if (isDragging) {
                const x = e.clientX - offsetX;
                const y = e.clientY - offsetY;
                element.style.left = `${x}px`;
                element.style.top = `${y}px`;
            }
        });

        document.addEventListener('mouseup', () => {
            isDragging = false;
            element.style.cursor = 'grab';
        });
    }


}
