#requires installation of mosquitto

mosquitto_sub -h ae78af72ecf943f6ae2cfbd6a4a4cf44.s1.eu.hivemq.cloud -p 8883 -u USERNAME -P PASSWORD -t my/test/topic
mosquitto_pub -h ae78af72ecf943f6ae2cfbd6a4a4cf44.s1.eu.hivemq.cloud -p 8883 -u YOUR_USERNAME -P YOUR_PASSWORD -t '/device/A/changePreferneces' -m '{"intervalMilliseconds": 1000, "unit": "Celcius"}'
