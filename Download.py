import sys
import requests

if len(sys.argv) < 2:
    raise IndexError

r = requests.get(sys.argv[1])

f = open(sys.argv[2], "wb")

f.write(r.content)

f.close()