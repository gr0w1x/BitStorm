const { NodeVM } = require("vm2");
const fs = require("fs");
const { fs: memfs } = require("memfs");
const chai = require("chai");
const path = require("path");

memfs.mkdirSync(process.cwd(), {
    recursive: true
});

const vm = new NodeVM({
    sandbox: {
        describe,
        it,
        before,
        beforeEach,
        after,
        afterEach
    },
    eval: false,
    wasm: false,
    allowAsync: true,
    console: "inherit",
    require: {
        external: ["chai"],
        mock: {
            fs: memfs,
            chai,
        }
    }
});

const code = fs.readFileSync(process.env.TEST, "utf-8");

vm.run(code);

const result = {
    Passed: 0,
    Failed: 0
}

afterEach(function () {
    var state = this.currentTest.state;
    if (state === "passed") {
        result.Passed += 1;
    } else if (state === "failed") {
        result.Failed += 1;
    }
});

after(function () {
    fs.writeFileSync(
        path.join(path.dirname(process.env.TEST), "results.json"),
        JSON.stringify(result)
    );
});
