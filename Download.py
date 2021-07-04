import sys
import requests

r = requests.get(sys.argv[1])

f = open(sys.argv[2], "wb")

f.write(r.content)

f.close()

print(f"Downloaded {sys.argv[2]}")