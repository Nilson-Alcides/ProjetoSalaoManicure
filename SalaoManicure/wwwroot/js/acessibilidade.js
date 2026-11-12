document.addEventListener("DOMContentLoaded", function () {

    const botao = document.getElementById("botaoAcessibilidade");
    const painel = document.getElementById("painelAcessibilidade");

    if (!botao || !painel) return;

    const chave = "salaoManicureAcessibilidade";

    let configuracoes = {
        fonte: 100,
        altoContraste: false,
        destacarLinks: false
    };

    // Recupera as preferências salvas
    try {
        const salvas = localStorage.getItem(chave);

        if (salvas) {
            configuracoes = {
                ...configuracoes,
                ...JSON.parse(salvas)
            };
        }
    } catch {
        console.warn("Não foi possível recuperar as preferências.");
    }

    function aplicarConfiguracoes() {
        document.documentElement.style.fontSize =
            configuracoes.fonte + "%";

        document.body.classList.toggle(
            "alto-contraste",
            configuracoes.altoContraste
        );

        document.body.classList.toggle(
            "destacar-links",
            configuracoes.destacarLinks
        );

        salvarConfiguracoes();
    }

    function salvarConfiguracoes() {
        try {
            localStorage.setItem(chave, JSON.stringify(configuracoes));
        } catch {
            console.warn("Não foi possível salvar as preferências.");
        }
    }

    // Abrir e fechar painel
    botao.addEventListener("click", function () {
        const aberto = !painel.hidden;

        painel.hidden = aberto;
        botao.setAttribute("aria-expanded", String(!aberto));
    });

    // Aumentar fonte
    document.getElementById("aumentarFonte")
        .addEventListener("click", function () {

            configuracoes.fonte = Math.min(
                configuracoes.fonte + 10,
                150
            );

            aplicarConfiguracoes();
        });

    // Diminuir fonte
    document.getElementById("diminuirFonte")
        .addEventListener("click", function () {

            configuracoes.fonte = Math.max(
                configuracoes.fonte - 10,
                80
            );

            aplicarConfiguracoes();
        });

    // Alto contraste
    document.getElementById("alternarContraste")
        .addEventListener("click", function () {

            configuracoes.altoContraste =
                !configuracoes.altoContraste;

            aplicarConfiguracoes();
        });

    // Destacar links
    document.getElementById("destacarLinks")
        .addEventListener("click", function () {

            configuracoes.destacarLinks =
                !configuracoes.destacarLinks;

            aplicarConfiguracoes();
        });

    // Restaurar configurações
    document.getElementById("restaurarAcessibilidade")
        .addEventListener("click", function () {

            configuracoes = {
                fonte: 100,
                altoContraste: false,
                destacarLinks: false
            };

            aplicarConfiguracoes();
        });

    // Fechar painel com ESC
    document.addEventListener("keydown", function (evento) {
        if (evento.key === "Escape" && !painel.hidden) {
            painel.hidden = true;
            botao.setAttribute("aria-expanded", "false");
            botao.focus();
        }
    });

    // Aplicar preferências ao carregar a página
    aplicarConfiguracoes();
});