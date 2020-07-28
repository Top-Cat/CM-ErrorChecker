function quadraticBezier(p0, p1, p2, p3, t) {
    var x =Math.pow(1-t, 3)*p0.x +
            Math.pow(1-t, 2)*3*t*p1.x +
            (1-t)*3*t*t*p2.x +
            t*t*t*p3.x
    var y =Math.pow(1-t, 3)*p0.y +
            Math.pow(1-t, 2)*3*t*p1.y +
            (1-t)*3*t*t*p2.y +
            t*t*t*p3.y
    var z =Math.pow(1-t, 3)*p0.z +
            Math.pow(1-t, 2)*3*t*p1.z +
            (1-t)*3*t*t*p2.z +
            t*t*t*p3.z
    return {x: x, y: y, z: z};
}

function generateCurve(walls, startPoint, p1, p2, endPoint) {
    var amount = 64;

    for (var i = 0; i < amount; i++) {
        var currentPoint = quadraticBezier(
            startPoint,
            p1,
            p2,
            endPoint,
            i / amount
        );

        var nextPoint = quadraticBezier(
            startPoint,
            p1,
            p2,
            endPoint,
            (i+1) / amount
        );

        var startRow = currentPoint.x;
        var startHeight = currentPoint.y;
        var startTime = Math.min(currentPoint.z, nextPoint.z);
        var width = Math.abs(nextPoint.x - currentPoint.x);
        var height = Math.abs(nextPoint.y - currentPoint.y);
        var duration = Math.abs(nextPoint.z - currentPoint.z);

        newWall = {
            _time: startTime,
            _lineIndex: 0,
            _type: 0,
            _duration: duration,
            _width: width,
            _customData: {
                _scale: [width, height],
                _position: [startRow, startHeight]
            }
        };

        walls.push(newWall);
    }
}

function performCheck(cursor, notes, events, walls, _, global, data) {
    log('Cursor = ' + cursor);
    log('Notes = ' + notes.length);
    log('Events = ' + events.length);
    log('Walls = ' + walls.length);

    while (notes.length > 5) {
        notes.shift();
    }

    while (notes.length < 10) {
        notes.push({_time: 1, _lineIndex: 1, _lineLayer: 1, _type: 1, _cutDirection: 1});
    }

    var i = 0;
    for (x in notes) {
        notes[x]._time = i++;
        notes[x]._lineIndex = Math.floor(Math.random() * 4);
        notes[x]._lineLayer = Math.floor(Math.random() * 3);
        notes[x]._type = Math.floor(Math.random() * 2);
        notes[x]._cutDirection = Math.floor(Math.random() * 9);
    }

    walls = [];

    generateCurve(
        walls,
        {x: 0, y: 0, z: 0},
        {x: 2, y: 0, z: 0.5},
        {x: 3, y: 0, z: 1},
        {x: 3, y: 2, z: 2}
    );

    generateCurve(
        walls,
        {x: 0, y: 0, z: 0},
        {x: -2, y: 0, z: 0.5},
        {x: -3, y: 0, z: 1},
        {x: -3, y: 2, z: 2}
    );

    generateCurve(
        walls,
        {x: 0, y: 3, z: 0},
        {x: 2, y: 3, z: 0.5},
        {x: 3, y: 3, z: 1},
        {x: 3, y: 1, z: 2}
    );

    generateCurve(
        walls,
        {x: 0, y: 3, z: 0},
        {x: -2, y: 3, z: 0.5},
        {x: -3, y: 3, z: 1},
        {x: -3, y: 1, z: 2}
    );

    return {walls: walls};
}

module.exports = {
    name: "Test",
    params: {},
    run: performCheck
};