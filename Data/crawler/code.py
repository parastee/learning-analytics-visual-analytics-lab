import requests
from bs4 import BeautifulSoup
import time
import json
import os

base_url = 'https://ranking.zeit.de/che/en/hochschule/'

def req(url):
    headers = {
        'authority': 'ranking.zeit.de',
        'pragma': 'no-cache',
        'cache-control': 'no-cache',
        'dnt': '1',
        'upgrade-insecure-requests': '1',
        'user-agent': 'Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36',
        'accept': 'text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9',
        'sec-fetch-site': 'none',
        'sec-fetch-mode': 'navigate',
        'sec-fetch-user': '?1',
        'sec-fetch-dest': 'document',
        'accept-language': 'en-US,en;q=0.9,de;q=0.8,fa;q=0.7',
        'cookie': 'JSESSIONID=0DEE910DE4A797B56BECA61B10AB56C9; _ga=GA1.3.884962878.1593446190; wt_nv=1; wt_nv_s=1; wt_fa=lv~1593446191245|1608998191245#cv~1|1608998191247#fv~1593446191248|1608998191248#; wtsid_981949533494636=1; wteid_981949533494636=4159344619100655421; _ga=GA1.2.2083383576.1593446238; creid=1670849482457920451; zeit_sso_201501=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpZCI6IjQ1ODI0NTciLCJlbWFpbCI6InNhbS5zb2x0YW5pQGdtYWlsLmNvbSIsIm5hbWUiOm51bGwsInN0YXRlIjoiYWN0aXZlIiwiY29udGFjdF92aWFfZW1haWwiOm51bGwsInJvbGVzIjpbXSwiaGFzX3Bhc3N3b3JkIjoidHJ1ZSIsImlkZW50aXRpZXMiOltdLCJleHAiOjE2MjQ5ODIyMzl9.BX283nb2luaIWDJ8mVoCiJzlyTG45iTuULD9UM-CojxvZtrE-vSpzTnCXD4NDTW3oy1ccDgbo27-XLaHwY0ERSrQU0OwBJcvMt6GcY6-_bDrKOdcrNpdPEEwee_zU_bVjK29GA2_piCgdstol473Vgsn66ICJjD6X20XN5IO4qup4YcaXwA5NAqgPSO-p8M3XyC9wXely9scIl2CrlsqEazFv6diPA4xIlFdBJR5d77qPzTxLkptmbqqfNYp8ykGmArFTYGGO6sfh0FN67O5ImhvfQAS5RnASVwkWGPp1CvPmAsFphwyXWsa0MKVqU-3u5vNean_3AIlAnPAWN_RMQ; cresession_migrated2=1593446239; POPUPCHECK=1593789132029; _gid=GA1.3.1762614343.1593702735; zeit_sso_session_201501=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpZCI6IjQ1ODI0NTciLCJlbWFpbCI6InNhbS5zb2x0YW5pQGdtYWlsLmNvbSIsIm5hbWUiOm51bGwsInN0YXRlIjoiYWN0aXZlIiwiY29udGFjdF92aWFfZW1haWwiOm51bGwsInJvbGVzIjpbIjEwOmFidGVzdF9wYXl3YWxsX2EiXSwiaGFzX3Bhc3N3b3JkIjoidHJ1ZSIsImlkZW50aXRpZXMiOltdLCJleHAiOjE1OTM3OTY3Njl9.aY810rfzDBPf1hIRrSg5uArZkDQq-ktjYsc8RDvlSe6sMBMbMtgZr_27quMDIiO5vBZoelJIqTcAaM8M4nZuF5gvgsW_r9sTEVAfw0ktuQaokEYf6mj7LmFQaHx-B7socRV9o2Hmj5pR5U61KVgTOo1dRi4uChWbsjuONlLIqsOJgUZpvVPzeVJVrElbVjwpd9_AZuQZ3TLeHdjtCxsFxhzGE4_0d0x-esOUdqGBybfGGy3efEBSkj1jnBs1b7MbJYOYfYfV2Mkvm4rOTGiLUMYekXfCxbNaqfjbOSUCaohXjiQube1T4wNECA1yWFR-P8Lg25-ogB7BwpGMwos_kg; _sp_enable_dfp_personalized_ads=true; zonconsent=2020-07-02T17:19:34.571Z; _gid=GA1.2.700480935.1593710376; c1_to_wt=1670849482457920451; ioam2018=001ab7b28df7d76985efa0f2c:1624031789115:1593446189115:.zeit.de:77:zeitonl:campus/bild-text:noevent:1593711129343:vsjn18; wt_fa_s=start~1|1625247130897#; wt_rla=981949533494636%2C56%2C1593709710678%3B821266747973781%2C47%2C1593709720829',
    }

    return requests.get(url, headers=headers)

def clean_label(text):
    return text.strip().lower() \
    .replace(' ', '_') \
    .replace("'", '') \
    .replace(",", "") \
    .replace("(", "") \
    .replace(")", "") \
    .replace("-", "_") \
    .replace(".", "") \

def clean_value(text):
    return text.replace('*', '') \
        .replace("\\r\\n", "") \
        .replace("\xa0", "") \
        .strip()

def get_active_uni():
    unis = []
    for i in range(1, 1100):
        r = requests.get(base_url + str(i))
        if r.status_code == 200:
            unis.append(i)

        time.sleep(1)

def get_info(soup):
    info = {}
    nodes = soup.find(class_='contentBlock').find_all(class_='trbg1')

    for node in nodes:
        label = None
        value = None
        if node.find(class_='checol1'):
            label = clean_label(node.find(class_='checol1').getText())
        if node.find(class_='checol2_rank'):
            value = node.find(class_='checol2_rank').getText()
        elif node.find(class_='checol2_balken'):
            value = node.find(class_='checol2_balken').getText()

        if label and value:
            info[label] = clean_value(value)

    return info

def get_universities(url):
    r = req(url)
    if r.status_code != 200:
        return

    soup = BeautifulSoup(r.text, features="html.parser")

    universities_nodes = soup.find_all(class_='selectUni')
    universities = []
    for node in universities_nodes:
        uni = node.find('a', href=True)
        universities.append({
            'name': uni.getText().strip(),
            'url': uni['href']
        })

    return universities

def get_program_info(url):
    r = req(url)
    if r.status_code != 200:
        return

    soup = BeautifulSoup(r.text, features="html.parser")

    program_info = {
        'program_name' : soup.find('h1').find(text=True, recursive=False).strip()
    }

    program_info.update(get_info(soup))

    return program_info

def get_department_info(url):
    r = req(url)
    if r.status_code != 200:
        return

    soup = BeautifulSoup(r.text, features="html.parser")

    department_info = {
        'department_name' : soup.find('h1').find(text=True, recursive=False).strip()
    }

    department_info.update(get_info(soup))

    degree_courses_nodes = soup.find_all(class_='contain-link', href=True)
    department_info['degree_courses'] = []
    for node in degree_courses_nodes:
        department_info['degree_courses'].append({
            'name' : node.getText().strip(),
            'url' : node['href']
        })

    return department_info


def get_uni_info(url):
    r = req(url)
    if r.status_code != 200:
        return

    soup = BeautifulSoup(r.text, features="html.parser")
    info = {}
    info['id'] = os.path.basename(url)
    info['name'] = soup.find('h1').getText().strip()
    info.update(get_info(soup))
    info['url'] = url
    info['departments'] = ''
    info['departments_detail'] = []
    departments_nodes = soup.find(class_='chelist-arrowBullets').find_all('li')
    for node in departments_nodes:
        dep_title = node.find(class_='contain-link', href=True).attrs['title']
        dep_extra = node.find(class_='additionalInfo')
        dep_link = node.find(class_='contain-link', href=True).attrs['href']

        info['departments'] += dep_title

        if dep_extra:
            info['departments'] += ' (' + dep_extra.getText() +  ')'
            dep_extra = dep_extra.getText()
        else:
            dep_extra = ''

        info['departments'] += ', '

        info['departments_detail'].append({
            'name' : dep_title,
            'extra': dep_extra,
            'url': dep_link
        })

    info['departments'] = info['departments'].strip(', ')
    return info

def save_unis_info():
    text_file = open("./data/uni-ids.txt", "r")
    unis_id = text_file.read().strip('\n').split('\n')

    unis_info = []
    for uni_id in unis_id:
        uni_info = get_uni_info(base_url + str(uni_id))
        if uni_info:
            print(uni_id, uni_info['name'])
            unis_info.append(uni_info)

        with open('./data/unis-info.json', 'w', encoding='utf-8') as f:
            json.dump(unis_info, f, ensure_ascii=False, indent=4)

        time.sleep(1)

def save_unis_departments_info():
    with open('./data/unis-info.json') as f:
        unis_info = json.load(f)

    unis_departments_info = []
    for uni in unis_info:
        for dep in uni['departments_detail']:
            department_info = get_department_info(dep['url'])
            if department_info:
                print(uni['name'], '-', department_info['department_name'])
                uni_departments_info = { 'name' : uni['name'] }
                uni_departments_info.update(department_info)
                uni_departments_info['url'] = dep['url']

                unis_departments_info.append(uni_departments_info)

                with open('./data/unis-departments-info.json', 'w', encoding='utf-8') as f:
                    json.dump(unis_departments_info, f, ensure_ascii=False, indent=4)

            time.sleep(1)

def save_unis_programs_info():
    with open('./data/unis-departments-info.json') as f:
        unis_departments_info = json.load(f)

        unis_programs_info = []
        for dep in unis_departments_info:
            for program in dep['degree_courses']:
                program_info = get_program_info(program['url'])

                if program_info:
                    print(dep['name'], '-', dep['department_name'],'-', program_info['program_name'])
                    uni_program_info = {}
                    uni_program_info['name'] = dep['name']
                    uni_program_info['department_name'] = dep['department_name']
                    uni_program_info.update(program_info)
                    uni_program_info['url'] = program['url']

                    unis_programs_info.append(uni_program_info)

                    with open('./data/unis-programs-info.json', 'w', encoding='utf-8') as f:
                        json.dump(unis_programs_info, f, ensure_ascii=False, indent=4)

                time.sleep(1)



