document.addEventListener('DOMContentLoaded', () => {
    const myButton = document.getElementById('btnClickMe') as HTMLButtonElement;

    if (myButton) {
        myButton.addEventListener('click', () => {
            alert("Кнопка нажата, а данные взяли из TS!");
            console.log('Логи в браузере');
        })
    }
})

function greet(name: string): void {
    console.log(`Привет, ${name} Это тестовый вызом ТайпСкрипта!`)
}