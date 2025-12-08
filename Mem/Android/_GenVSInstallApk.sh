cd "$(dirname "$0")"

rm HoleDiggingSimulator.apks

bundletool build-apks --connected-device --bundle=HoleDiggingSimulator.aab --output HoleDiggingSimulator.apks --ks=HoleDiggingSimulator.keystore --ks-pass=pass:xgamest@123 --ks-key-alias=hole_digging_simulator --key-pass=pass:xgamest@123

bundletool install-apks --apks=HoleDiggingSimulator.apks
