# ChroMapper Error Checker

Simple plugin that adds an error checker to [ChroMapper](https://github.com/Caeden117/ChroMapper)

### Installation

Download the latest release, place the entire folder in your `ChroMapper/Plugins` directory

### Built-in plugins

- Vision Blocks
  - Copy of the logic in MMA2's error checker
  - Finds blocks that are obscured by blocks in the center two positions
- Stacked Blocks
  - Finds blocks that may be overlapping
  - Looks for notes less than 0.1 beats of each other in the same position

### JS Checks

You can add more error check types directly by placing javascript files in the same directory as the plugin.

The parser uses [Jint 3 (beta)](https://github.com/sebastienros/jint/issues/343) and [esprima](https://github.com/sebastienros/esprima-dotnet) which can parse ES6 but not all features are supported, for instance esprima can parse `class` but Jint will throw an exception so you'll have to rewrite these as old-style functions and prototypes

The script made for beatmap v2 can be used for beatmap v3 and vice versa. However, `customData` require special handling by the script to assign correct key for specific version.

#### Module

This part of the code defines the script to be shown and used in editor. This is currently designed to be compatible with MM scripts, you just need to add the name and params. As this is essentially alpha, the interface for these checks may change but for now you just need to create a block similar to:

```js
module.exports = {
	name: "My error check",
	params: { "Min Time": 0.24 },
	run: (cursor, notes, events, walls, _, global, data, customEvents, bpmChanges, bombs, arcs, chains) => {},
	errorCheck: true
};
```

- `name` string appears in the drop-down list.
- `params` is an object, for each key a text field will be show to the user where they can enter a value which will be passed to `run` function
  - The object is only allowed to have set of certain type of value:
    - `"stringKey": "string"`
    - `"numberKey": 0.0`
    - `"booleanKey": true`
    - `"arrayKey": ["ary"]` (only array of string is allowed)
- `run` is a function that will be called when the user runs your check.
- `errorCheck` is an optional boolean property to display error message, default to `true`.

#### Run Function

The function uses the data from current active beatmap. You can directly modify the value of beatmap objects in the array with code like `notes[0]._time = 10;` **OR** if you want to generate a fresh array you need to provide your new array in object returned from your function (see below).

```js
function run(cursor, notes, events, walls, _, global, data, customEvents, bpmChanges, bombs, arcs, chains) {}
```

  - `cursor` is the current beat time of grid cursor in editor.
  - `notes` is an array of objects (v3 will not include bomb note).
  - `events` is an array of events (v3 will not include boost and rotation event).
  - `walls` is an array of obstacles.
  - `_` In MM scripts the parameter here was called save? I don't know what it did but this parameter is just an empty object and has no use. Provided for compatibility.
  - `globals` can be used to persist data between runs of your script, it will be unchanged on future invocations.
    - `params` is included as an object which contains the values set for your params.
      - The value set for params is `number`, `string` and `boolean`.
      - It can be accessed via string index `global.params.key` or number index `global.params[0]` (based on the order of `module.exports.params`).
      - Property assignment is not possible and re-assignment while allowed is not recommended, assign the param value to new variable is recommended if called often.
  - `data` has read-only information about the map, currently the list of data is as follows:
    - `currentBPM` is the BPM at the cursor accounting for BPM changes.
    - `songBPM` is the BPM of the song.
    - `NJS` is the note jump speed on current difficulty.
    - `offset` is the offset of NJS on current difficulty.
    - `characteristic` is current beatmap characteristics.
    - `difficulty` is current beatmap difficulty.
    - `environment` is current beatmap environment.
    - `version` is current beatmap version.
  - `customEvents` is an array of custom objects used for [noodling](https://github.com/Aeroluna/NoodleExtensions/blob/master/Documentation/AnimationDocs.md#custom-events)
  - `bpmChanges` may be useful for working out what the bpm is at a point in a map or programatically creating slides.
  - `bombs` **V3 ONLY** is an array of bomb notes.
  - `arcs` **V3 ONLY** is an array of arcs.
  - `chains` **V3 ONLY** is an array of chains.

Two functions will be defined before calling `run` or `performCheck`

- `addError(note, reason)` pass back the problem note object, all it's properties (except `customData`) must match the original passed note for it to be marked properly and you can provide a reason as the second parameter.
- `addWarning(note, reason)` is the same as `addError` except the note will only be highlighted yellow.

[There are example scripts in Examples](Examples)

![Example](example.png)
