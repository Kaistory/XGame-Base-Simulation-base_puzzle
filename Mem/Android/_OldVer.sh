cd "$(dirname "$0")"

rm OldVer.apks

bundletool build-apks --connected-device --bundle=OldVer.aab --output OldVer.apks --ks=SuperMarket.keystore --ks-pass=pass:xgamest@123 --ks-key-alias=supermarket_simulator --key-pass=pass:xgamest@123

bundletool install-apks --apks=OldVer.apks
