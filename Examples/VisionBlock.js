function performCheck(cursor, notes, events, walls, _, global, data) {
	minTime = global.params[0];
	maxTime = global.params[1];
	visionBlockLeft = -1;
	visionBlockRight = -1;

	if (notes.length > 0) {
		visionBlockLeftNote = notes[0];
		visionBlockRightNote = notes[0];
	}

	for (x in notes) {
		note = notes[x];

		if (note._time - visionBlockLeft <= maxTime) {
			if (note._lineIndex < 2 && note._time - visionBlockLeft > minTime) {
				addError(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
			}

			if (note._lineLayer == 1 && note._lineIndex == 1) {
				addError(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
			}
		}

		if (note._time - visionBlockRight <= maxTime) {
			if (note._lineIndex > 1 && note._time - visionBlockRight > minTime) {
				addError(visionBlockRightNote, "Blocks vision of upcoming note on the right");
			}

			if (note._lineLayer == 1 && note._lineIndex == 2 && note._time - visionBlockLeft <= maxTime) {
				addError(visionBlockRightNote, "Blocks vision of upcoming note on the right");
			}
		}

		if (note._type != 3 && note._lineLayer == 1) {
			if (note._lineIndex == 1) {
				visionBlockLeft = note._time;
				visionBlockLeftNote = note;
			} else if (note._lineIndex == 2) {
				visionBlockRight = note._time;
				visionBlockRightNote = note;
			}
		}
	}
}

module.exports = {
	name: "Vision Blocks",
	params: {"Min Time": 0.24, "Max Time": 0.75},
	run: performCheck
};