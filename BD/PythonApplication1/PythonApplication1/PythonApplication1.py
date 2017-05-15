import pypyodbc
import json
from datetime import datetime

keyword_actions = [('Abandon'),
("Activate"),
("Ante"),
("Assemble"),
("Attach"),
("Ban"),
("Bolster"),
("Bury"),
("Cast"),
("Clash"),
("Counter (keyword action)"),
("Destroy"),
("Detain"),
("Discard"),
("Exchange"),
("Exile"),
("Fateseal"),
("Fight"),
("Investigate"),
("Manifest"),
("Meld"),
("Monstrosity"),
("Planeswalk"),
("Play"),
("Populate"),
("Proliferate"),
("Regeneration"),
("Reveal"),
("Sacrifice"),
("Scry"),
("Search"),
("Set in motion"),
("Shuffle"),
("Support"),
("Tap"),
("Transform"),
("Untap"),
("Vote")]

keyword_abilities = [("Deathtouch"),
("Defender"),
("Double Strike"),
("Enchant"),
("Equip"),
("First Strike"),
("Flash"),
("Flying"),
("Haste"),
("Hexproof"),
("Indestructible"),
("Lifelink"),
("Menace"),
("Prowess"),
("Reach"),
("Trample"),
("Vigilance"),
("Absorb"),
("Affinity"),
("Amplify"),
("Annihilator"),
("Aura Swap"),
("Awaken"),
("Banding"),
("Battle Cry"),
("Bestow"),
("Bloodthirst"),
("Bushido"),
("Buyback"),
("Cascade"),
("Champion"),
("Changeling"),
("Cipher"),
("Conspire"),
("Convoke"),
("Cumulative Upkeep"),
("Cycling"),
("Dash"),
("Delve"),
("Dethrone"),
("Devoid"),
("Devour"),
("Dredge"),
("Echo"),
("Entwine"),
("Epic"),
("Evoke"),
("Evolve"),
("Exalted"),
("Exploit"),
("Extort"),
("Fading"),
("Fear"),
("Flanking"),
("Flashback"),
("Forecast"),
("Fortify"),
("Frenzy"),
("Fuse"),
("Graft"),
("Gravestorm"),
("Haunt"),
("Hidden Agenda"),
("Hideaway"),
("Horsemanship"),
("Infect"),
("Ingest"),
("Intimidate"),
("Kicker"),
("Landhome"),
("Landwalk"),
("Level Up"),
("Living Weapon"),
("Madness"),
("Megamorph"),
("Miracle"),
("Modular"),
("Morph"),
("Myriad"),
("Ninjutsu"),
("Offering"),
("Outlast"),
("Overload"),
("Persist"),
("Phasing"),
("Poisonous"),
("Protection"),
("Provoke"),
("Prowl"),
("Rampage"),
("Rebound"),
("Recover"),
("Reinforce"),
("Renown"),
("Replicate"),
("Retrace"),
("Ripple"),
("Scavenge"),
("Skulk"),
("Shadow"),
("Shroud"),
("Soulbond"),
("Soulshift"),
("Splice"),
("Split Second"),
("Storm"),
("Substance"),
("Sunburst"),
("Surge"),
("Suspend"),
("Totem Armor"),
("Transfigure"),
("Transmute"),
("Tribute"),
("Undying"),
("Unearth"),
("Unleash"),
("Vanishing"),
("Wither")]

ConnectionString = r"DRIVER={ODBC Driver 13 for SQL Server};SERVER=DESKTOP-H3IDCPD\SQLEXPRESS;Database=Magic;Trusted_Connection=yes"

conn = pypyodbc.connect(ConnectionString)
c = conn.cursor()

SQL = 'SELECT * FROM <YOURTABLE>'

c.execute('USE Magic')
c.execute("SELECT * FROM [User]")

f = open(r"C:\Users\David\Desktop\AllSets.json", "r", encoding="utf8")

magic_data = json.loads(f.read())

f.close()




for f in magic_data:

    
    set = magic_data[f]

    
    releaseDate = datetime.strptime(set['releaseDate'], "%Y-%m-%d").date()

    if releaseDate >= datetime.strptime('2015-10-1', "%Y-%m-%d").date():
        legality = 'Standard'

    elif releaseDate >= datetime.strptime('2003-07-28', "%Y-%m-%d").date():
            legality = 'Modern'

    else:
        legality = 'Vintage'
    

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
    
    
    cards = set['cards'];
    print(set['name'])
    for card in cards:
        for param in ['artist', 'imageName', 'cmc', 'name', 'rarity', 'text', 'manaCost', 'multiverseid']:
            if param not in card.keys():
                card[param] = None
 
        c.execute("Insert INTO Card (artist, imageName, cmc, manaCost, multiverseID, name, rarity, text, edition) values (?, ?, ?, ?, ?, ?, ?, ?, ?)", [card['artist'], card['imageName'], card['cmc'], card['manaCost'],card['multiverseid'], card['name'], card['rarity'], card['text'], set['code']])
        c.execute("select @@IDENTITY")
        
        cardID = int(c.fetchone()[0])

        if card['text'] is not None:
            for keyword_abilitie in keyword_abilities:
                if keyword_abilitie in card['text']:
                    c.execute("Insert INTO Ability (Card, Ability, Action) Values (?, ?, ?)", [cardID, keyword_abilitie, 0])
        
            for keyword_action in keyword_actions:
                if keyword_action in card['text']:
                    c.execute("Insert INTO Ability (Card, Ability, Action) Values (?, ?, ?)", [cardID, keyword_action, 1])

        if 'colorIdentity' in card.keys():
            for color in card['colorIdentity']:
                if card['manaCost'] is not None and color in card['manaCost']:
                    c.execute('Insert INTO ColorIdentity VALUES (?,?,?)', [cardID, color, 1])
                else:
                    c.execute('Insert INTO ColorIdentity VALUES (?,?,?)', [cardID, color, 0])

        if 'types' in card.keys():
            for type in card['types']:
                 c.execute('Insert INTO TypeOfCard VALUES (?,?)', [cardID, type])

        if 'subtypes' in card.keys():
            for type in card['subtypes']:
                 c.execute('Insert INTO SubtypeOfCard VALUES (?,?)', [cardID, type])

        if 'power' in card.keys():
            if '*' in card['power']:
                card['power'] = None
            else:
                card['power'] = int(float(card['power']))

            if '*' in card['toughness']:
                card['toughness'] = None
            else:
                card['toughness'] = int(float(card['toughness']))

            c.execute('Insert INTO Creature VALUES (?,?,?)', [cardID, card['power'], card['toughness']])

        if 'flavor' in card.keys():
            c.execute('Insert INTO Flavor VALUES (?,?)', [cardID, card['flavor']])
        

c.commit();
            
