"""Serve Unity Web builds by path."""
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
from urllib.parse import urlsplit
import argparse

ROOT = Path(__file__).resolve().parent
WORLDS = {"Town_Sunny", "Town_Rain"}

class UnityHandler(SimpleHTTPRequestHandler):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, directory=str(SERVE_DIR), **kwargs)

    def permitted(self):
        parts = [part for part in urlsplit(self.path).path.split('/') if part]
        return bool(parts) and parts[0] in WORLDS and all(part not in ('.', '..') and '%' not in part and '\\' not in part for part in parts)

    def do_GET(self):
        if MENU and not self.permitted():
            self.send_error(404)
        else:
            super().do_GET()

    def do_HEAD(self):
        if MENU and not self.permitted():
            self.send_error(404)
        else:
            super().do_HEAD()

    def end_headers(self):
        if urlsplit(self.path).path.endswith('.gz') and self._headers_buffer[0].startswith(b'HTTP/1.0 200'):
            self.send_header('Content-Encoding', 'gzip')
        super().end_headers()

    def guess_type(self, path):
        for suffix, mime in (('.wasm.gz', 'application/wasm'), ('.js.gz', 'application/javascript'), ('.data.gz', 'application/octet-stream')):
            if path.endswith(suffix):
                return mime
        return super().guess_type(path)

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Serve Unity worlds locally.')
    parser.add_argument('folder', nargs='?', choices=sorted(WORLDS), help='Serve one world directly')
    args = parser.parse_args()
    MENU = args.folder is None
    SERVE_DIR = ROOT / args.folder if args.folder else ROOT
    if args.folder and not (SERVE_DIR / 'index.html').is_file():
        parser.error(f'index.html not found in {SERVE_DIR}')
    print(f'http://127.0.0.1:8765/  ({SERVE_DIR})', flush=True)
    ThreadingHTTPServer(('127.0.0.1', 8765), UnityHandler).serve_forever()
