import os
from os import path
from glob import glob
import subprocess

homedir = path.dirname(__file__)

if os.getcwd() is not homedir:
    os.chdir(homedir)

files = glob("*.szs")

Sarc_cs = glob("*\\*.exe")[3]

for file in files:
    subprocess.call(f'{Sarc_cs} {file}')

    files = glob("*.byml")

    Byml_cs = glob("*\\*.exe")[0]

    for file in files:
        subprocess.call(f'{Byml_cs} {file}')
        os.remove(file)

    Dumper = glob("*\\*.exe")[2]

    files = glob("*.yml")

    for file in files:
        subprocess.call(f'{Dumper} {file}')
        os.remove(file)

    Counter = glob('*\\*.exe')[1]

    files = glob("*.txt")

    for file in files:
        subprocess.call(f'{Counter} {file}')