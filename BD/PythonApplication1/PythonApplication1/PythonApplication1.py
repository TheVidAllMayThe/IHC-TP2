import pypyodbc
import json
from datetime import datetime

ConnectionString = r"DRIVER={ODBC Driver 13 for SQL Server};SERVER=DESKTOP-H3IDCPD\SQLEXPRESS;Database=Magic;Trusted_Connection=yes"

conn = pypyodbc.connect(ConnectionString)
c = conn.cursor()

SQL = 'SELECT * FROM <YOURTABLE>'

c.execute('USE Magic')
c.execute("SELECT * FROM [User]")

f = open(r"C:\Users\David\Desktop\AllSets.json", "r", encoding="utf8")

print(c.fetchall())

magic_data = json.loads(f.read())

f.close()

print(len(magic_data))



for f in magic_data:

    
    set = magic_data[f]

    """ADD SETS    

    releaseDate = datetime.strptime(set['releaseDate'], "%Y-%m-%d").date()

    if releaseDate >= datetime.strptime('2015-10-1', "%Y-%m-%d").date():
        legality = 'Standard'

    elif releaseDate >= datetime.strptime('2003-07-28', "%Y-%m-%d").date():
            legality = 'Modern'

    else:
        legality = 'Vintage'
    
    print(set['name'])

    try:
      c.execute("Insert INTO Edition (name, code, gathererCode, releaseDate, legality, mkm_id) values (?, ?, ?, ?, ?, ?)", [set['name'], set['code'], set['gathererCode'], set['releaseDate'], legality, set['mkm_id']])
    except KeyError:
      try:
         c.execute("Insert INTO Edition (name, code, releaseDate, legality, mkm_id) values (?, ?, ?, ?, ?)", [set['name'], set['code'], set['releaseDate'], legality, set['mkm_id']])
      except KeyError: 
        try: 
           c.execute("Insert INTO Edition (name, code, gathererCode, releaseDate, legality) values (?, ?, ?, ?, ?)", [set['name'], set['code'],set['gathererCode'], set['releaseDate'], legality])
        except KeyError:
           c.execute("Insert INTO Edition (name, code, releaseDate, legality) values (?, ?, ?, ?)", [set['name'], set['code'], set['releaseDate'], legality])
    """
    
    cards = set[cards];

    for card in set[cards]:
    
    
    
c.commit();
            
