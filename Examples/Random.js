function performCheck(data) {
	notes = data.notes;
	for (x in notes) {
		note = notes[x];

		if (Math.random() > 0.95) {
			addError(note, "");
		} else if (Math.random() > 0.95) {
			addWarning(note, "");
		}
	}
}

module.exports = {
	name: "Random",
	params: {},
	performCheck: performCheck
};