"""Serve the local Unity Web build with the headers required for gzip files."""
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path


class UnityHandler(SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(Path(__file__).parent / "Town_WebGL1_Build"), **kwargs)

    def end_headers(self):
        if self.path.split("?", 1)[0].endswith(".gz"):
            self.send_header("Content-Encoding", "gzip")
        super().end_headers()

    def guess_type(self, path):
        if path.endswith(".wasm.gz"):
            return "application/wasm"
        if path.endswith(".js.gz"):
            return "application/javascript"
        if path.endswith(".data.gz"):
            return "application/octet-stream"
        return super().guess_type(path)


if __name__ == "__main__":
    ThreadingHTTPServer(("127.0.0.1", 8765), UnityHandler).serve_forever()
